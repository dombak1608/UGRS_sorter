#include "funkcije.h"

void initSvega(void)
{
	filteriP[brCasa-1].boje = 0x1F;
	filteriP[brCasa-1].oblici = 0x1F;
	filteriP[brCasa-1].masaMin = 0x00;
	filteriP[brCasa-1].masaMax = 0x7F;

	//postavljanje motora na nultu poziciju (brcasa-1 je zadnja casa)
	while(HAL_GPIO_ReadPin(PHint1_GPIO_Port, PHint1_Pin) == GPIO_PIN_SET)
	{
		stepperMove(4, 3);
	}
	trenutnaCasa = 0;
}

void filterParseIzBuffer(void)
{
	uint8_t i;
	uint8_t flagNule = 0;	//sve nule
	uint8_t flagJedinice = 0;	//sve jedinice
	for(i = 0; i < brCasa - 1; i++)
	{
		if(rx_buffer[i] == 0)
		{
			flagNule++;
		}
		else if(rx_buffer[i] == 0xFF)
		{
			flagJedinice++;
		}
		else
		{
			break;
		}
	}
	if(flagNule == (brCasa-1))	//sve nule
	{
		receivedSettings = 1;	//stop
	}
	else if(flagJedinice == (brCasa-1))	//sve jedinice
	{
		receivedSettings = 3;	//analiza
	}
	else
	{
		for(i = 0; i < brCasa - 1; i++)
		{
			filteriP[i].oblici = 0;
			filteriP[i].boje = 0;
			filteriP[i].masaMin = 0;
			filteriP[i].masaMax = 0;
		}
		for (i = 0; i < brCasa - 1; i++)
		{
			filteriP[i].oblici = rx_buffer[0] & maskaF(5);	//uzimanje 5 bitova iz polja i pomicanje cijelog polja
			pomicanjePolja(5);

			filteriP[i].boje = rx_buffer[0] & maskaF(5);	//uzimanje 5 bitova iz polja i pomicanje cijelog polja
			pomicanjePolja(5);

			filteriP[i].masaMin = rx_buffer[0] & maskaF(7);	//uzimanje 7 bitova iz polja i pomicanje cijelog polja
			pomicanjePolja(7);

			filteriP[i].masaMax = rx_buffer[0] & maskaF(7);	//uzimanje 7 bitova iz polja i pomicanje cijelog polja
			pomicanjePolja(7);
		}
		receivedSettings = 2;
	}
}

void pomicanjePolja(uint8_t brBit)
{
	uint8_t i;
	for (i = 0; i < buffSize; i++)
	{
		rx_buffer[i] >>= brBit;
		if (i < buffSize - 1)
		{
			rx_buffer[i] |= (rx_buffer[i + 1] & maskaF(brBit)) << (8 - brBit);
		}
	}
}

uint8_t maskaF(uint8_t brBit)
{
	uint8_t i;
	uint8_t maska = 0;
	for (i = 0; i < brBit; i++)
	{
		maska <<= 1;
		maska |= 1;
	}
	return maska;
}

void HX711_Init()
{
	LC_struct.pinSck = LC_CK_Pin;
	LC_struct.pinData = LC_DT_Pin;
	LC_struct.gpioSck = LC_CK_GPIO_Port;
	LC_struct.gpioData = LC_DT_GPIO_Port;
	LC_struct.gain = 1;
	HAL_GPIO_WritePin(LC_struct.gpioSck, LC_struct.pinSck, GPIO_PIN_SET);
	HAL_Delay(1);
	HAL_GPIO_WritePin(LC_struct.gpioData, LC_struct.pinData, GPIO_PIN_RESET);
}

uint32_t HX711_Average_Value(uint8_t times)
{
    uint32_t sum = 0;
    for (int i = 0; i < times; i++)
    {
        sum += HX711_Value();
    }

    return sum / times;
}

uint32_t HX711_Value()
{
    uint32_t buffer = 0;
    uint8_t i;

    while (HAL_GPIO_ReadPin(LC_struct.gpioData, LC_struct.pinData)==1);

    for (i = 0; i < 24; i++)
    {
    	HAL_GPIO_WritePin(LC_struct.gpioSck, LC_struct.pinSck, GPIO_PIN_SET);

        buffer = buffer << 1 ;

        if (HAL_GPIO_ReadPin(LC_struct.gpioData, LC_struct.pinData))
        {
            buffer ++;
        }

        HAL_GPIO_WritePin(LC_struct.gpioSck, LC_struct.pinSck, GPIO_PIN_RESET);
    }

    for (i = 0; i < LC_struct.gain; i++)
    {
    	HAL_GPIO_WritePin(LC_struct.gpioSck, LC_struct.pinSck, GPIO_PIN_SET);
    	HAL_GPIO_WritePin(LC_struct.gpioSck, LC_struct.pinSck, GPIO_PIN_RESET);
    }

    buffer = buffer ^ 0x800000;

    return buffer;
}

double HX711_GramGet(uint8_t numTimes)
{
	int32_t b = HX711_Average_Value(numTimes) - LC_struct.offset;
	double rez = (double) b * (-1.0);
	rez *= 0.0010443;	//iz karakteristike vage
	return (rez - LC_struct.y_offset);
}

void HX711_Tare(uint8_t times)
{
	LC_struct.offset = HX711_Average_Value(times);
	LC_struct.y_offset = 0.0;
	LC_struct.y_offset = HX711_GramGet(times);
}

void stepperMove(uint16_t brKoraka, uint8_t brSteppera)
{
	uint16_t i;

	switch(brSteppera)
	{
	case 1:
		for(i = 0; i<brKoraka; i++)
		{
			HAL_GPIO_WritePin(getPort(1,4), getPin(1,4), GPIO_PIN_RESET);
			HAL_GPIO_WritePin(getPort(1,1), getPin(1,1), GPIO_PIN_SET);
			HAL_GPIO_WritePin(getPort(1,2), getPin(1,2), GPIO_PIN_RESET);
			HAL_GPIO_WritePin(getPort(1,3), getPin(1,3), GPIO_PIN_RESET);
			HAL_Delay(3);
			HAL_GPIO_TogglePin(getPort(1,1), getPin(1,1));
			HAL_GPIO_TogglePin(getPort(1,2), getPin(1,2));
			HAL_Delay(3);
			HAL_GPIO_TogglePin(getPort(1,2), getPin(1,2));
			HAL_GPIO_TogglePin(getPort(1,3), getPin(1,3));
			HAL_Delay(3);
			HAL_GPIO_TogglePin(getPort(1,3), getPin(1,3));
			HAL_GPIO_TogglePin(getPort(1,4), getPin(1,4));
			HAL_Delay(3);
		}
		break;
	case 2:
		for(i = 0; i<brKoraka; i++)
		{
			HAL_GPIO_WritePin(getPort(2,4), getPin(2,4), GPIO_PIN_RESET);
			HAL_GPIO_WritePin(getPort(2,1), getPin(2,1), GPIO_PIN_SET);
			HAL_GPIO_WritePin(getPort(2,2), getPin(2,2), GPIO_PIN_RESET);
			HAL_GPIO_WritePin(getPort(2,3), getPin(2,3), GPIO_PIN_RESET);
			HAL_Delay(3);
			HAL_GPIO_TogglePin(getPort(2,1), getPin(2,1));
			HAL_GPIO_TogglePin(getPort(2,2), getPin(2,2));
			HAL_Delay(3);
			HAL_GPIO_TogglePin(getPort(2,2), getPin(2,2));
			HAL_GPIO_TogglePin(getPort(2,3), getPin(2,3));
			HAL_Delay(3);
			HAL_GPIO_TogglePin(getPort(2,3), getPin(2,3));
			HAL_GPIO_TogglePin(getPort(2,4), getPin(2,4));
			HAL_Delay(3);
		}
		break;
	case 3:
		for(i = 0; i<brKoraka; i++)
		{
			HAL_GPIO_WritePin(getPort(3,4), getPin(3,4), GPIO_PIN_RESET);
			HAL_GPIO_WritePin(getPort(3,1), getPin(3,1), GPIO_PIN_SET);
			HAL_GPIO_WritePin(getPort(3,2), getPin(3,2), GPIO_PIN_RESET);
			HAL_GPIO_WritePin(getPort(3,3), getPin(3,3), GPIO_PIN_RESET);
			HAL_Delay(3);
			HAL_GPIO_TogglePin(getPort(3,1), getPin(3,1));
			HAL_GPIO_TogglePin(getPort(3,2), getPin(3,2));
			HAL_Delay(3);
			HAL_GPIO_TogglePin(getPort(3,2), getPin(3,2));
			HAL_GPIO_TogglePin(getPort(3,3), getPin(3,3));
			HAL_Delay(3);
			HAL_GPIO_TogglePin(getPort(3,3), getPin(3,3));
			HAL_GPIO_TogglePin(getPort(3,4), getPin(3,4));
			HAL_Delay(3);
		}
		break;
	default:
		break;
	}

}

void stepperMoveNeg(uint16_t brKoraka, uint8_t brSteppera)
{
	uint16_t i;
	uint8_t delBr=2;
	switch(brSteppera)
		{
		case 1:
			for(i = 0; i<brKoraka; i++)
			{
				HAL_GPIO_WritePin(getPort(1,4), getPin(1,4), GPIO_PIN_RESET);
				HAL_GPIO_WritePin(getPort(1,2), getPin(1,2), GPIO_PIN_RESET);
				HAL_GPIO_WritePin(getPort(1,1), getPin(1,1), GPIO_PIN_SET);
				HAL_GPIO_WritePin(getPort(1,3), getPin(1,3), GPIO_PIN_RESET);
				HAL_Delay(delBr);
				HAL_GPIO_TogglePin(getPort(1,1), getPin(1,1));
				HAL_GPIO_TogglePin(getPort(1,4), getPin(1,4));
				HAL_Delay(delBr);
				HAL_GPIO_TogglePin(getPort(1,4), getPin(1,4));
				HAL_GPIO_TogglePin(getPort(1,3), getPin(1,3));
				HAL_Delay(delBr);
				HAL_GPIO_TogglePin(getPort(1,3), getPin(1,3));
				HAL_GPIO_TogglePin(getPort(1,2), getPin(1,2));
				HAL_Delay(delBr);
			}
			break;
		case 2:
			for(i = 0; i<brKoraka; i++)
			{
				HAL_GPIO_WritePin(getPort(2,4), getPin(2,4), GPIO_PIN_RESET);
				HAL_GPIO_WritePin(getPort(2,2), getPin(2,2), GPIO_PIN_RESET);
				HAL_GPIO_WritePin(getPort(2,1), getPin(2,1), GPIO_PIN_SET);
				HAL_GPIO_WritePin(getPort(2,3), getPin(2,3), GPIO_PIN_RESET);
				HAL_Delay(delBr);
				HAL_GPIO_TogglePin(getPort(2,1), getPin(2,1));
				HAL_GPIO_TogglePin(getPort(2,4), getPin(2,4));
				HAL_Delay(delBr);
				HAL_GPIO_TogglePin(getPort(2,4), getPin(2,4));
				HAL_GPIO_TogglePin(getPort(2,3), getPin(2,3));
				HAL_Delay(delBr);
				HAL_GPIO_TogglePin(getPort(2,3), getPin(2,3));
				HAL_GPIO_TogglePin(getPort(2,2), getPin(2,2));
				HAL_Delay(delBr);
			}
			break;
		case 3:
			for(i = 0; i<brKoraka; i++)
			{
				HAL_GPIO_WritePin(getPort(3,4), getPin(3,4), GPIO_PIN_RESET);
				HAL_GPIO_WritePin(getPort(3,2), getPin(3,2), GPIO_PIN_RESET);
				HAL_GPIO_WritePin(getPort(3,1), getPin(3,1), GPIO_PIN_SET);
				HAL_GPIO_WritePin(getPort(3,3), getPin(3,3), GPIO_PIN_RESET);
				HAL_Delay(delBr);
				HAL_GPIO_TogglePin(getPort(3,1), getPin(3,1));
				HAL_GPIO_TogglePin(getPort(3,4), getPin(3,4));
				HAL_Delay(delBr);
				HAL_GPIO_TogglePin(getPort(3,4), getPin(3,4));
				HAL_GPIO_TogglePin(getPort(3,3), getPin(3,3));
				HAL_Delay(delBr);
				HAL_GPIO_TogglePin(getPort(3,3), getPin(3,3));
				HAL_GPIO_TogglePin(getPort(3,2), getPin(3,2));
				HAL_Delay(delBr);
			}
			break;
		default:
			break;
		}
}

void stopiranje()	//namjestitit case u pocetnu poziciju
{
	//postavljanje motora na nultu poziciju (brcasa-1 je zadnja casa)
	while(HAL_GPIO_ReadPin(PHint1_GPIO_Port, PHint1_Pin) == GPIO_PIN_SET)
	{
		stepperMove(4, 3);
	}
	trenutnaCasa = 0;
}

void sortiranje()
{
	uint8_t numberOfSpins = 0;
	while(HX711_GramGet(2) < minMasaSort)
	{
		stepperMoveNeg(20, 1);	//spinstepper1 za neki kut
		if(++numberOfSpins > maxStepper1Spins || receivedSettings != 2)	//zaustavljanje rada
		{
			receivedSettings = 0;
			if((ukupniElementi % 2) == 0)
			{
				getShapeAndColor(1);
			}
			else
			{
				getShapeAndColor(0);
			}

			setStepper3ToPosition(getPositionFromFilter(!(ukupniElementi % 2)));

			stepperMoveNeg(158, 2);	//spinstepper2 za neki kut (90+45-45)-element na sredini kruga
			stepperMove(30, 2);
			HX711_Tare(5);	//tariranje vage, na vagi nema nista
			sendResultToUart(ukupniElementi);	//info o elem
			ukupniElementi = 0;
			HAL_Delay(100);
			sendResultToUart(0xFF);	//zavrsetak rada
			break;
		}
	}
	if(receivedSettings == 2)
	{
		ukupniElementi++;
		if((ukupniElementi % 2) == 0)
		{
			objekti[0].weight = (uint8_t)HX711_GramGet(4);
			getShapeAndColor(1);
		}
		else
		{
			objekti[1].weight = (uint8_t)HX711_GramGet(4);
			getShapeAndColor(0);
		}
		if(ukupniElementi>1)
		{
			setStepper3ToPosition(getPositionFromFilter(!(ukupniElementi % 2)));
			sendResultToUart(ukupniElementi-1);
		}
		stepperMoveNeg(158, 2);	//spinstepper2 za neki kut (90+45-45)-element na sredini kruga
		stepperMove(30, 2);
		HX711_Tare(5);	//tariranje vage, na vagi nema nista
	}
}

void analiziranje()
{
	uint8_t numberOfSpins = 0;
	while(HX711_GramGet(2) < minMasaSort)
	{
		stepperMoveNeg(20, 1);	//spinstepper1 za neki kut
		if(++numberOfSpins > maxStepper1Spins || receivedSettings != 3)	//zaustavljanje rada
		{
			receivedSettings = 0;
			if((ukupniElementi % 2) == 0)
			{
				getShapeAndColor(1);
			}
			else
			{
				getShapeAndColor(0);
			}

			if((ukupniElementi%3) == 0)
			{
				stepperMove(86, 3);
			}
			else
			{
				stepperMove(85, 3);
			}

			stepperMoveNeg(158, 2);	//spinstepper2 za neki kut (90+45-45)-element na sredini kruga
			stepperMove(30, 2);
			HX711_Tare(5);	//tariranje vage, na vagi nema nista
			sendResultToUart(ukupniElementi);
			ukupniElementi = 0;
			HAL_Delay(100);
			sendResultToUart(0xFF); //da je gotov
			break;
		}
	}
	if(receivedSettings == 3)
	{
		ukupniElementi++;
		if((ukupniElementi % 2) == 0)
		{
			objekti[0].weight = (uint8_t)HX711_GramGet(4);
			getShapeAndColor(1);
		}
		else
		{
			objekti[1].weight = (uint8_t)HX711_GramGet(4);
			getShapeAndColor(0);
		}
		if(ukupniElementi > 2)
		{
			if((ukupniElementi%3)==0)
			{
				stepperMove(86, 3);
			}
			else
			{
				stepperMove(85, 3);
			}
		}
		if(ukupniElementi > 1)
		{
			sendResultToUart(ukupniElementi-1);
		}
		stepperMoveNeg(158, 2);	//spinstepper2 za neki kut (90+45-45)-element na sredini kruga
		stepperMove(30, 2);
		HX711_Tare(5);	//tariranje vage, na vagi nema nista
	}
}

void sendResultToUart(uint8_t brElement)
{
	uint8_t bufferSl[4];
	bufferSl[0] = brElement;
	bufferSl[1] = objekti[!(ukupniElementi%2)].shape;
	bufferSl[2] = objekti[!(ukupniElementi%2)].color;
	bufferSl[3] = objekti[!(ukupniElementi%2)].weight;
	HAL_UART_Transmit(&huart2, bufferSl, 4 ,50);
}

void getShapeAndColor(uint8_t brCycle)
{
	rx_bufferPI[0]=0;
	rx_bufferPI[1]=0;
	rx_bufferPI[2]=0;
	while(rx_bufferPI[0] != 3 || rx_bufferPI[1] == 3 || rx_bufferPI[2] == 3 || rx_bufferPI[1] == 0 || rx_bufferPI[2] == 0)
	{
		HAL_UART_Receive(&huart1, rx_bufferPI, 3, 300);
	}

	objekti[brCycle].shape = rx_bufferPI[1];
	objekti[brCycle].color = rx_bufferPI[2];
}

uint8_t getPositionFromFilter(uint8_t brCycle)
{
	uint8_t i;
	for(i = 0; i < brCasa; i++)
	{
		if((filteriP[i].oblici & objekti[brCycle].shape) && (filteriP[i].boje & objekti[brCycle].color) && (filteriP[i].masaMin < objekti[brCycle].weight) && (filteriP[i].masaMax > objekti[brCycle].weight))
		{
			return i;
		}
	}
	return (brCasa-1);
}

void setStepper3ToPosition(uint8_t brCase)
{
	if((brCase - trenutnaCasa) == 0)
	{
		return 0;
	}
	else if(abs(brCase - trenutnaCasa) == 3)
	{
		stepperMoveNeg(256, 3);
	}
	else if(abs(brCase - trenutnaCasa) < 3 && (brCase - trenutnaCasa) < 0)
	{
		if(abs(brCase - trenutnaCasa) == 1)
		{
			stepperMoveNeg(85, 3);
		}
		else
		{
			stepperMoveNeg(171, 3);
		}
	}
	else if(abs(brCase - trenutnaCasa) < 3 && (brCase - trenutnaCasa) > 0)
	{
		if(abs(brCase - trenutnaCasa) == 1)
		{
			stepperMove(85, 3);
		}
		else
		{
			stepperMove(171, 3);
		}
	}
	else if(abs(brCase - trenutnaCasa) > 3 && (brCase - trenutnaCasa) < 0)
	{
		if(abs(brCase - trenutnaCasa) == 5)
		{
			stepperMove(85, 3);
		}
		else
		{
			stepperMove(171, 3);
		}
	}
	else if(abs(brCase - trenutnaCasa) > 3 && (brCase - trenutnaCasa) > 0)
	{
		if(abs(brCase - trenutnaCasa) == 5)
		{
			stepperMoveNeg(85, 3);
		}
		else
		{
			stepperMoveNeg(171, 3);
		}
	}

	trenutnaCasa = brCase;
}
