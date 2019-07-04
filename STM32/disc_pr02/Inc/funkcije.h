#ifndef __FUNKCIJE_H
#define __FUNKCIJE_H

#ifdef __cplusplus
extern "C" {
#endif

#include "main.h"

#define getPort(X,Y) Step##X##Y##_GPIO_Port
#define getPin(X,Y) Step##X##Y##_Pin

#define minMasaSort 3.0
#define maxStepper1Spins 100

typedef struct filterS
{
	uint8_t oblici;
	uint8_t boje;
	uint8_t masaMin;
	uint8_t masaMax;
}filter;

typedef struct _hx711
{
	GPIO_TypeDef* gpioSck;
	GPIO_TypeDef* gpioData;
	uint16_t pinSck;
	uint16_t pinData;
	int offset;
	float y_offset;
	int gain;
	// 1: channel A, gain factor 128
	// 2: channel B, gain factor 32
    // 3: channel A, gain factor 64
}HX711;

typedef struct _objectInfo
{
	uint8_t weight;
	uint8_t shape;
	uint8_t color;
}objektInfo;

extern UART_HandleTypeDef huart2;
extern UART_HandleTypeDef huart1;

extern volatile uint8_t receivedSettings;
extern uint8_t tx_buffer[buffSize];
extern uint8_t rx_buffer[buffSize];

extern uint8_t rx_bufferPI[3];

extern HX711 LC_struct;
extern filter filteriP[brCasa];
extern objektInfo objekti[2];
extern uint8_t trenutnaCasa;
extern uint8_t ukupniElementi;

void initSvega(void);
void filterParseIzBuffer(void);
void pomicanjePolja(uint8_t brBit);
uint8_t maskaF(uint8_t brBit);

void HX711_Init(void);
uint32_t HX711_Average_Value(uint8_t times);
uint32_t HX711_Value(void);
void HX711_Tare(uint8_t times);
double HX711_GramGet(uint8_t numTimes);
void LCbuffer(void);

void stepperMove(uint16_t brKoraka, uint8_t brSteppera);
void stepperMoveNeg(uint16_t brKoraka, uint8_t brSteppera);

void stopiranje();
void sortiranje();
void analiziranje();
void getShapeAndColor(uint8_t brCycle);
uint8_t getPositionFromFilter(uint8_t brCycle);
void setStepper3ToPosition(uint8_t brCase);
void sendResultToUart(uint8_t brElementa);

#endif
