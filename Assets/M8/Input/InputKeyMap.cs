using UnityEngine;
using System.Collections;

//custom key mapping
public enum InputKeyMap : int {
	None = -1,

#if OUYA
    LX,
    LY,
    RX,
    RY,

    DPADX,
    DPADY,

    UP,
    DOWN,
    LEFT,
    RIGHT,

    O,
    U,
    Y,
    A,

    SYSTEM,
#endif

    NumKeys
}