#include <SoftwareSerial.h>
#include "ardprintf.h"
SoftwareSerial RFID(2, 3); // RX, TX

#define PWM_L_PIN 5
#define PWM_R_PIN 6

#define MOTOR_L_PIN1 9
#define MOTOR_L_PIN2 10

#define MOTOR_R_PIN1 7
#define MOTOR_R_PIN2 8

#define DISTANCE_SENSOR_PIN A3
#define VOLTAGE_SENSOR_PIN A2
#define GROUND_SENSOR1_PIN A0
#define GROUND_SENSOR2_PIN A1

#define HEADER "X"
#define CRC_9 0x31

//teoretycznie obrót o 180*
//#define ROTATE_TIME_R 1.12
//#define ROTATE_TIME_L 1.26

//teoretycznie obrót o 90*
//#define ROTATE_TIME_R 0.48
//#define ROTATE_TIME_L 0.51

//teoretycznie obrót o 45*
//#define ROTATE_TIME_R 0.14
//#define ROTATE_TIME_L 0.16

//zaleznosci czas[sekundy](kąt[stopnie])
//przyblożone na podst 3pkt -> niedokładne XD
// ROTATE_TIME_R = (0.0072* KĄT - 0.18)
// ROTATE_TIME_L = (00082* KĄT - 0.215)

#define FAST_START_PWM 240

byte CRC8;
char pDataFrame[50];
char buf[50];
bool TIMER2_FLAG = 0;

int timer1 = 0;
bool flag1 = false;
int timer2 = 0;


void setup() {

//przerwanie z timera2 co 2kHz
  TCCR2A = 0;
  TCCR2B = 0;
  TCNT2  = 0;
  OCR2A = 255;
  TCCR2A |= (1 << WGM21);
  TCCR2B |= (1 << CS21);   
  TIMSK2 |= (1 << OCIE2A);
  
  Serial.begin(115200);
  RFID.begin(9600);

  pinMode(13, OUTPUT);
  
  pinMode(MOTOR_L_PIN1, OUTPUT);
  pinMode(MOTOR_L_PIN2, OUTPUT);
  pinMode(MOTOR_R_PIN1, OUTPUT);
  pinMode(MOTOR_R_PIN2, OUTPUT);

  digitalWrite(MOTOR_L_PIN1, LOW);
  digitalWrite(MOTOR_L_PIN2, LOW);
  digitalWrite(MOTOR_R_PIN1, LOW);
  digitalWrite(MOTOR_R_PIN2, LOW);

  analogWrite(PWM_L_PIN, 0);
  analogWrite(PWM_R_PIN, 0);
  analogReference(INTERNAL);
  delay(2000);
  sei();
}

//funkscja przerwania co 2kHz
ISR(TIMER2_COMPA_vect){
  TIMER2_FLAG = 1;
//  int distL, distR; 
//  get_distance_ground(distL, distR);
//  if( analogRead(DISTANCE_SENSOR_PIN) > 700){
//  go_stop();
// }
}

void go_straight(){
  //na chwilę duży PWM, żeby ładnie wystrartował
  //przy mnijszym pwm jeden silnik za wolno startuje
  //co powoduje skręcanie
  analogWrite(PWM_L_PIN, FAST_START_PWM);
  analogWrite(PWM_R_PIN, FAST_START_PWM-4);
  digitalWrite(MOTOR_L_PIN1, LOW);
  digitalWrite(MOTOR_L_PIN2, HIGH);
  digitalWrite(MOTOR_R_PIN1, LOW);
  digitalWrite(MOTOR_R_PIN2, HIGH);
  
  delay(100);
  
  analogWrite(PWM_L_PIN, 150);
  analogWrite(PWM_R_PIN, 150-15);
}

void go_back(){
  //na chwilę duży PWM, żeby ładnie wystrartował
  //przy mnijszym pwm jeden silnik za wolno startuje
  //co powoduje skręcanie
  analogWrite(PWM_L_PIN, FAST_START_PWM);
  analogWrite(PWM_R_PIN, FAST_START_PWM-4);
  digitalWrite(MOTOR_L_PIN1, HIGH);
  digitalWrite(MOTOR_L_PIN2, LOW);
  digitalWrite(MOTOR_R_PIN1, HIGH);
  digitalWrite(MOTOR_R_PIN2, LOW);
  
  delay(100);
  
  analogWrite(PWM_L_PIN, 150);
  analogWrite(PWM_R_PIN, 150-15);
}

void go_stop(){
  analogWrite(PWM_L_PIN, 255);
  analogWrite(PWM_R_PIN, 255);

  digitalWrite(MOTOR_L_PIN1, LOW);
  digitalWrite(MOTOR_L_PIN2, LOW);
  digitalWrite(MOTOR_R_PIN1, LOW);
  digitalWrite(MOTOR_R_PIN2, LOW);
  delay(100);
}

void rotateR(byte angle){
  //na chwilę duży PWM, żeby ładnie wystrartował
  //przy mnijszym pwm jeden silnik za wolno startuje
  //co powoduje skręcanie
  analogWrite(PWM_L_PIN, FAST_START_PWM);
  analogWrite(PWM_R_PIN, FAST_START_PWM);
   //W prawo
  digitalWrite(MOTOR_L_PIN1, HIGH);
  digitalWrite(MOTOR_L_PIN2, LOW);
  digitalWrite(MOTOR_R_PIN1, LOW);
  digitalWrite(MOTOR_R_PIN2, HIGH);
  delay(50);
  
  analogWrite(PWM_L_PIN, 100);
  analogWrite(PWM_R_PIN, 100);

  double ROTATE_TIME_R = 0.0072* angle - 0.18;
  if(ROTATE_TIME_R < 0) ROTATE_TIME_R == 0;
  delay(ROTATE_TIME_R*1000);
  go_stop();
  flag1 = false;
  Serial.print("3\r\n");
}

void rotateL(byte angle){
  //na chwilę duży PWM, żeby ładnie wystrartował
  //przy mnijszym pwm jeden silnik za wolno startuje
  //co powoduje skręcanie
  analogWrite(PWM_L_PIN, FAST_START_PWM);
  analogWrite(PWM_R_PIN, FAST_START_PWM);
  //W lewo
  digitalWrite(MOTOR_L_PIN1, LOW);
  digitalWrite(MOTOR_L_PIN2, HIGH);
  digitalWrite(MOTOR_R_PIN1, HIGH);
  digitalWrite(MOTOR_R_PIN2, LOW);
  
  delay(100);

  analogWrite(PWM_L_PIN, 100);
  analogWrite(PWM_R_PIN, 100);
  
  double ROTATE_TIME_L = 0.0082 * angle - 0.215;
  if(ROTATE_TIME_L < 0) ROTATE_TIME_L == 0;
  delay(ROTATE_TIME_L*1000);
  go_stop();
  Serial.print("4\r\n");
}

int get_distance(){
  return analogRead(DISTANCE_SENSOR_PIN);
}

void get_distance_ground(int &distance1, int &distance2){
  distance1 = analogRead(GROUND_SENSOR1_PIN);
  distance2 = analogRead(GROUND_SENSOR2_PIN);
}

double get_voltage(){
  return analogRead(VOLTAGE_SENSOR_PIN)*(1.1/1024.0)*7.5; // dla dzielnika 47k i 6.8k max napiecie 8,7V
}

bool check_base(){
  byte inByte[17];
  RFID.readBytes(inByte, 17);
  if(inByte[0]== '\n' && inByte[1]== '\r' && inByte[2]=='S' && inByte[3]=='N' && inByte[4]==' ' && inByte[5]=='0' && inByte[6]=='x'  && inByte[7]=='0'  && inByte[8]=='3'  && inByte[9]=='C'  && inByte[10]=='6' && inByte[11]=='2' && inByte[12]=='7' && inByte[13]=='0' && inByte[14]=='0'  && inByte[15]=='3'  && inByte[16]=='2' ){
    for(int i = 0; i < 17; i++ ) inByte[i] = 0;
    return 1;
  }
  return 0;
}
/*
byte CRC8_DataArray(byte *pData, byte Len){
    byte CRC_final = 0xff;

    for(int i = 0; i < Len; ++i){
        CRC_final = CRC8_SingleByte(CRC_final, pData[i]);
    }
return CRC_final;
}

byte CRC8_SingleByte(byte CRC_prev, byte Data){
    CRC_prev ^= Data;
    for(int i = 0; i < 8; ++i){
        if((CRC_prev & 0x80) != 0){
            CRC_prev = (byte)((CRC_prev << 1) ^ CRC_9);}
        else{
            CRC_prev = (byte)(CRC_prev << 1);}
    }
    return CRC_prev;
}

String readCRC(String pData){
    pData.substring(0,pData.length()-9).toCharArray(buf, 50);
    byte t_CRC8 = CRC8_DataArray((unsigned char*)buf, strlen(buf));
    //Serial.print(pData);  
    //Serial.println(buf);
    if(String((int)t_CRC8, HEX) == pData.substring(pData.length()-8,pData.length()-6) && pData.substring(0,1) == String(HEADER)){
      return pData.substring(2,pData.length()-9);
    }
    else return String("Incorrect CRC");
}
*/

void autopilot(){
  /*
  distL - lewy czujnik podłoza (jeśli jest wieksze niz 1000 to jest krawedz
  distR - prawy czujnik podloza
  ktorys z tych czujnikow podloza nie dziala wiec ich moze nie uzywaj

  get_distance() - czujnik z przodu
  check_base() - czy jest w stacji dokowania
  */
  int dis = 850;
  bool isGoing = false;
  int iterator = 0;
  while(true){
    if(dis > get_distance() && isGoing == false){
      go_straight();
      isGoing = true;
      //iterator++;
    }
    
    if(dis < get_distance()){
      if(isGoing == true){
        go_stop();
        isGoing = false;
        delay(100);
        rotateL(160);
      }
      else{
        rotateL(30);
        delay(100);
      }
    }
    /*
    if(iterator != 0 && iterator % 3 == 0){
      if(dis == 1000){
        dis = 600;
      }
      else if(dis == 600){
        break;
      }
    }*/
    delay(100);
  }
}

void loop() {
  //proste wykonywanie poleceń
  byte inByte[3] = {'0','0','0'};
  if(Serial.available()){
    Serial.readBytes(inByte, 3);
    if(inByte[0]== '0' && inByte[1]== '\r' && inByte[2]=='\n' ){
      go_stop();
    }
    if(inByte[0]== '1' && inByte[1]== '\r' && inByte[2]=='\n' ){
      go_straight();
    }
    if(inByte[0]== '2' && inByte[1]== '\r' && inByte[2]=='\n' ){
      go_back();
    }
    if(inByte[0]== '3' && inByte[1]== '\r' && inByte[2]=='\n' ){
      rotateR(90);
    }
    if(inByte[0]== '4' && inByte[1]== '\r' && inByte[2]=='\n' ){
      rotateL(90);
    }
    if(inByte[0]== '5' && inByte[1]== '\r' && inByte[2]=='\n' ){
      autopilot();
    }
    if(inByte[0]== '6' && inByte[1]== '\r' && inByte[2]=='\n' ){
      //jakies przerwanie autopilota
    }
  }
  int distL, distR;
  get_distance_ground(distL, distR);


  if(millis() - timer2 >= 100){
    Serial.print("S "); //lewy czujnik podloza
    Serial.print(distL);
    Serial.print(" "); //prawy czujnik podloza
    Serial.print(distR);
    Serial.print(" "); //czujnik odleglosci
    Serial.print(get_distance());
    Serial.print(" "); //napiecie baterii
    Serial.print(get_voltage());
    Serial.print(" "); //wykrycie bazy
    Serial.println(check_base());
    timer2 = millis();
  }

  
  //proste odbijanie się od ścian
//    if(get_distance() > 600){
//      go_stop();
//      go_back();
//      delay(1000);
//      go_stop();
//      rotateR(90);
//      go_stop();
//    }else go_straight();


  
//****************************************************NIE KASOWAĆ BEGIN
//
//  int distL, distR;
//  get_distance_ground(distL, distR);
//
//  Serial.print("lewy czujnik podloza: ");
//  Serial.print(distL);
//  Serial.print(" prawy czujnik podloza: ");
//  Serial.print(distR);
//  Serial.print(" czujnik odleglosci: ");
//  Serial.print(get_distance());
//  Serial.print(" napiecie baterii: ");
//  Serial.print(get_voltage());
//  Serial.print(" wykrycie bazy: ");
//  Serial.println(check_base());
//  delay(200);

//  go_straight();
//  delay(2000);
//  go_stop(); //hard breaking - go_stop(255), easy breaking - go_stop(0)
//  rotateL(90);
//  go_stop();
//
//  go_straight();
//  delay(2000);
//  go_stop(); //hard breaking - go_stop(255), easy breaking - go_stop(0)
//  rotateR(90);
//  go_stop();
  
  //go_straight();
  
//  delay(2000);
//  rotateL(100);
  
  //distL > 950 || distR > 950 ||get_distance() > 100 ||
//  if(  get_distance() > 700){
//     rotateR(100);
//     delay(500);
//  }
//  go_straight(150);
//  delay(1000);
  //delay(100);

 // rotate(255);

 //****************************************************NIE KASOWAĆ END
  
 /***KOMUNIKACJA Z RPI*****/
/*  sprintf(pDataFrame, "%s %d %d", HEADER, 100, -200); //string do wyslania
  CRC8 = CRC8_DataArray((unsigned char*)pDataFrame, strlen(pDataFrame)); //obliczanie crc8
  ardprintf("%s %x \r\n", (char*)pDataFrame, (int)CRC8); //wysylanie po uart
  Serial.println(readCRC(Serial.readString())); //odczyt danych ze stringu w argumencie  */
  
}
