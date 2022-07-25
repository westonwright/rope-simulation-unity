using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void SetStickPosition1Delegate(ref RopeStick s, ref Vector3 stickCenter, ref Vector3 stickDirection, bool order, int stickIndex = 0);

public delegate void SetStickPosition2Delegate(ref RopeStick s, ref Vector3 stickCenter, ref Vector3 stickDirection, bool order);
