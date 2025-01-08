using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public sealed class AnimatorMessageHandler : MonoBehaviour {
    public Action<string> OnMessageGetted = (message) => { /*Debug.Log(message);*/ };

    public void NewMessage(string message) {
        OnMessageGetted(message);
    }
}
