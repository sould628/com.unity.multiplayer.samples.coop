using System;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.Assertions;


namespace Unity.Multiplayer.Samples.BossRoom.Editor
{
    public class NetworkLatencyWarning : MonoBehaviour
    {
        Text m_LatencyText;
        bool m_LatencyTextCreated;

        Color m_TextColor = Color.red;

        bool m_ArtificialLatencyEnabled;

        void Update()
        {
            if (NetworkManager.Singleton != null && (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer))
            {
                var unityTransport = (UnityTransport) NetworkManager.Singleton.NetworkConfig.NetworkTransport;

                // adding this preprocessor directive check since UnityTransport's simulator tools only inject latency in #UNITY_EDITOR or in #DEVELOPMENT_BUILD
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                var simulatorParameters = unityTransport.DebugSimulator;
                m_ArtificialLatencyEnabled = simulatorParameters.PacketDelayMS > 0 ||
                    simulatorParameters.PacketJitterMS > 0 ||
                    simulatorParameters.PacketDropRate > 0;
#else
                m_ArtificialLatencyEnabled = false;
#endif

                if (m_ArtificialLatencyEnabled)
                {
                    if (!m_LatencyTextCreated)
                    {
                        m_LatencyTextCreated = true;
                        CreateLatencyText();
                    }

                    m_TextColor.a = Mathf.PingPong(Time.time, 1f);
                    m_LatencyText.color = m_TextColor;
                }
            }
            else
            {
                m_ArtificialLatencyEnabled = false;
            }

            if (!m_ArtificialLatencyEnabled)
            {
                if (m_LatencyTextCreated)
                {
                    m_LatencyTextCreated = false;
                    Destroy(m_LatencyText);
                }
            }
        }

        // Creating a UI text object and add it to NetworkOverlay canvas
        void CreateLatencyText()
        {
            Assert.IsNotNull(NetworkOverlay.Instance,
                "No NetworkOverlay object part of scene. Add NetworkOverlay prefab to bootstrap scene!");

            var statUI = new GameObject("UI Latency Warning Text");

            m_LatencyText = statUI.AddComponent<Text>();
            m_LatencyText.font = Font.CreateDynamicFontFromOSFont("Arial", 24);
            m_LatencyText.horizontalOverflow = HorizontalWrapMode.Overflow;
            m_LatencyText.alignment = TextAnchor.MiddleLeft;
            m_LatencyText.raycastTarget = false;
            m_LatencyText.resizeTextForBestFit = true;

            m_LatencyText.text = "Network Latency Enabled";

            var statUIRectTransform = statUI.GetComponent<RectTransform>();
            NetworkOverlay.Instance.AddToUI(statUIRectTransform);
        }
    }
}