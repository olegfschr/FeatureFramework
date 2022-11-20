// (c) 2022 Oleg Fischer

using System.Collections.Generic;
using System.Reflection;
using MessageSystem;
using UnityEngine;

namespace Features
{
	public class FeatureManager : MonoBehaviour, IMessageReceiver
	{
		private static List<Feature> _features;
		private static List<Feature> _featuresNeedUpdate;

		private int _featuresInitialised;

		private void Awake()
		{
			_features = new List<Feature>();
			
			System.Type[] types = Assembly.GetAssembly(typeof(Feature)).GetTypes();
			foreach (var feature in types)
			{
				if (feature.BaseType != typeof(Feature) || feature.IsAbstract) continue;
				
				_features.Add((Feature)System.Activator.CreateInstance(feature));
			}
			
			_featuresNeedUpdate = new List<Feature>(_features.Count);

			_featuresInitialised = _features.Count;
		}

		private void Start()
		{
			MessageManager.StartReceivingMessage<OnFeatureInitialisedMessage>(this);
			
			foreach (var feature in _features)
			{
				feature.Init();
				if (feature.NeedsUpdate())
				{
					_featuresNeedUpdate.Add(feature);
				}
			}
		}

		private void Update()
		{
			foreach (var feature in _featuresNeedUpdate)
			{
				feature.Update();
			}
		}

		private void OnDestroy()
		{
			foreach (var feature in _features)
			{
				feature.Cleanup();
			}
		}

		/// <summary>
		/// Destroys the feature and, if needed, removes it from the update list.
		/// </summary>
		/// <param name="feature">Feature to be removed.</param>
		public static void RemoveFeature(Feature feature)
		{
			if (!_features.Contains(feature)) return;
			
			feature.Cleanup();
			_features.Remove(feature);
				
			if (feature.NeedsUpdate() && _featuresNeedUpdate.Contains(feature))
			{
				_featuresNeedUpdate.Remove(feature);
			}
			
		}

		public void MessageReceived(Message message)
		{
			switch (message)
			{
				case OnFeatureInitialisedMessage:
					_featuresInitialised--;

					if (_featuresInitialised == 0)
					{
						MessageManager.StopReceivingMessage<OnFeatureInitialisedMessage>(this);
						MessageManager.ClearMessageCache<OnFeatureInitialisedMessage>();
						MessageManager.GetMessage<StartGameMessage>().Send();
					}
					break;
			}
			
			message.OnDoneUsing();
		}
	}
	
}