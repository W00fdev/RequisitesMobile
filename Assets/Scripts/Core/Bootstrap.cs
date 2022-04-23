using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public sealed class Bootstrap
    {
        [RuntimeInitializeOnLoadMethod]
        private static void Initializer()
        {
            bool debuggingInfo = false;

            if (debuggingInfo)
            {
                Debug.Log("LOG INITIALIZING --------------");

                switch (Application.platform)
                {
                    case RuntimePlatform.Android:
                        Debug.Log("Android platform");
                        break;
                    case RuntimePlatform.IPhonePlayer:
                        Debug.Log("IPhone platform");
                        break;
                    case RuntimePlatform.WindowsPlayer:
                        Debug.Log("Windows platform");
                        break;
                    default:
                        Debug.Log("Unknown platform");
                        break;
                }

                Debug.Log("Device Model: " + SystemInfo.deviceModel);
                Debug.Log("Device Name: " + SystemInfo.deviceName);     // Android can deal problems with it.
                Debug.Log("OS Type: " + SystemInfo.operatingSystem);
                Debug.Log("Graphics device name: " + SystemInfo.graphicsDeviceName);
                Debug.Log("Processor count: " + SystemInfo.processorCount);
                Debug.Log("UID: " + SystemInfo.deviceUniqueIdentifier);

                if (Application.genuineCheckAvailable && Application.genuine == false)
                    Debug.Log("Hacked version");

                Debug.Log("LOG INITIALIZED --------------");

            }

            Application.wantsToQuit += WantsToQuit;
            Application.quitting += Quit;
        }

        private static bool WantsToQuit()
        {
            Debug.Log("Trying to quit.");
            return true;
        }

        private static void Quit()
        {
            Debug.Log("Before the quit.");
        }
    }
}

