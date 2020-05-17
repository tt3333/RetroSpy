#define NOT_CONNECTED	   0

// PORT D
#define DIGITAL_PIN_02	   0
#define DIGITAL_PIN_14	   1
#define DIGITAL_PIN_07	   2
#define DIGITAL_PIN_08	   3
#define DIGITAL_PIN_06	   4
#define DIGITAL_PIN_20	   5
#define DIGITAL_PIN_21	   6
#define DIGITAL_PIN_05	   7
// PORT B
#define DIGITAL_PIN_16	   0
#define DIGITAL_PIN_27	   1
#define DIGITAL_PIN_19	   2
#define DIGITAL_PIN_18	   3

// Used for MODE_SELECT determination
#define DIGITAL_PIN_33	   33
#define DIGITAL_PIN_34	   34
#define DIGITAL_PIN_35	   35
#define DIGITAL_PIN_36	   36
#define DIGITAL_PIN_37	   37

#define MODEPIN_SNES       DIGITAL_PIN_33
#define MODEPIN_N64        DIGITAL_PIN_34
#define MODEPIN_GC         DIGITAL_PIN_35
#define MODEPIN_DREAMCAST  DIGITAL_PIN_36
#define MODEPIN_WII        DIGITAL_PIN_37

#define N64_PIN	           DIGITAL_PIN_02

#define SNES_LATCH         DIGITAL_PIN_08
#define SNES_DATA          DIGITAL_PIN_06
#define SNES_CLOCK         DIGITAL_PIN_21

#define NES_LATCH          DIGITAL_PIN_08
#define NES_CLOCK          DIGITAL_PIN_21
#define NES_DATA           DIGITAL_PIN_06
#define NES_DATA0          DIGITAL_PIN_07
#define NES_DATA1          DIGITAL_PIN_20

#define GC_PIN             DIGITAL_PIN_20

#define ThreeDO_LATCH      NOT_CONNECTED
#define ThreeDO_DATA       NOT_CONNECTED
#define ThreeDO_CLOCK      NOT_CONNECTED

#define PCFX_LATCH         NOT_CONNECTED
#define PCFX_CLOCK         NOT_CONNECTED
#define PCFX_DATA          NOT_CONNECTED

#define SS_SELECT0         NOT_CONNECTED
#define SS_SEL             NOT_CONNECTED
#define SS_SELECT1         NOT_CONNECTED
#define SS_REQ             NOT_CONNECTED
#define SS_ACK             NOT_CONNECTED
#define SS_DATA0           NOT_CONNECTED
#define SS_DATA1           NOT_CONNECTED
#define SS_DATA2           NOT_CONNECTED
#define SS_DATA3           NOT_CONNECTED

#define TG_SELECT          NOT_CONNECTED
#define TG_DATA1           NOT_CONNECTED
#define TG_DATA2           NOT_CONNECTED
#define TG_DATA3           NOT_CONNECTED
#define TG_DATA4           NOT_CONNECTED

#define PS_ATT             NOT_CONNECTED
#define PS_CLOCK           NOT_CONNECTED
#define PS_ACK             NOT_CONNECTED
#define PS_CMD             NOT_CONNECTED
#define PS_DATA            NOT_CONNECTED

//PORTD
#define NEOGEO_SELECT      NOT_CONNECTED
#define NEOGEO_D           NOT_CONNECTED
#define NEOGEO_B           NOT_CONNECTED
#define NEOGEO_RIGHT       NOT_CONNECTED
#define NEOGEO_DOWN        NOT_CONNECTED
#define NEOGEO_START       NOT_CONNECTED
//PORTB
#define NEOGEO_C           NOT_CONNECTED
#define NEOGEO_A           NOT_CONNECTED
#define NEOGEO_LEFT        NOT_CONNECTED
#define NEOGEO_UP          NOT_CONNECTED

#define INTPIN1            NOT_CONNECTED
#define INTPIN2            NOT_CONNECTED
#define INTPIN3            NOT_CONNECTED
#define INTPIN4            NOT_CONNECTED
#define INTPIN5			   NOT_CONNECTED
#define INTPIN6            NOT_CONNECTED
#define INTPIN7            NOT_CONNECTED
#define INTPIN8            NOT_CONNECTED
#define INTPIN9            NOT_CONNECTED

#define AJ_COLUMN1         NOT_CONNECTED
#define AJ_COLUMN2         NOT_CONNECTED
#define AJ_COLUMN3		   NOT_CONNECTED
#define AJ_COLUMN4         NOT_CONNECTED

#define CD32_LATCH    DIGITAL_PIN_20 
#define CD32_DATA     DIGITAL_PIN_05  
#define CD32_CLOCK    DIGITAL_PIN_21  

#define SMS_INPUT_PIN_0     7
#define SMS_INPUT_PIN_1     8
#define SMS_INPUT_PIN_2     6
#define SMS_INPUT_PIN_3    20
#define SMS_INPUT_PIN_4     5
#define SMS_INPUT_PIN_5    16

#define SMSONG_INPUT_PIN_0     7
#define SMSONG_INPUT_PIN_1     8
#define SMSONG_INPUT_PIN_2     6 
#define SMSONG_INPUT_PIN_3    20
#define SMSONG_INPUT_PIN_4    21
#define SMSONG_INPUT_PIN_5     5

#define READ_PORTD( mask ) (GPIOD_PDIR & mask)
#define READ_PORTB( mask ) (GPIOB_PDIR & mask)

#define PIND_READ( pin )  ((READ_PORTD(0xFF))&(1<<(pin)))
#define PINB_READ( pin )  ((READ_PORTB(0xF))&(1<<(pin)))
#define PINC_READ( pin )  (digitalReadFast(pin))

#define MICROSECOND_NOPS "nop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\n"

#define T_DELAY( ms ) delay(ms)
#define A_DELAY
