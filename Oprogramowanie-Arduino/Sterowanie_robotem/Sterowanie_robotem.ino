#define PWM_L_PIN 5
#define PWM_R_PIN 6

#define MOTOR_L_PIN1 7
#define MOTOR_L_PIN2 8

#define MOTOR_R_PIN1 9
#define MOTOR_R_PIN2 10

#define DISTANCE_SENSOR_PIN A0
#define GROUND_SENSOR1_PIN A1
#define GROUND_SENSOR2_PIN A2

void setup() {
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
}

void go_straight(byte PWM_Speed){
  analogWrite(PWM_L_PIN, PWM_Speed);
  analogWrite(PWM_R_PIN, PWM_Speed);

  digitalWrite(MOTOR_L_PIN1, LOW);
  digitalWrite(MOTOR_L_PIN2, HIGH);
  digitalWrite(MOTOR_R_PIN1, LOW);
  digitalWrite(MOTOR_R_PIN2, HIGH);
}

void go_back(byte PWM_Speed){
  analogWrite(PWM_L_PIN, PWM_Speed);
  analogWrite(PWM_R_PIN, PWM_Speed);

  digitalWrite(MOTOR_L_PIN1, HIGH);
  digitalWrite(MOTOR_L_PIN2, LOW);
  digitalWrite(MOTOR_R_PIN1, HIGH);
  digitalWrite(MOTOR_R_PIN2, LOW);
}

int get_distance(){
  return analogRead(DISTANCE_SENSOR_PIN);
}

void get_distance_ground(int &distance1, int &distance2){
  distance1 = analogRead(GROUND_SENSOR1_PIN);
  distance2 = analogRead(GROUND_SENSOR2_PIN);
}


void loop() {
  

}
