using System;
using UnityEngine;
using UnityEngine.Events;

namespace Modern2D
{

	//used for variables that don't need to be updated every frame
	[System.Serializable]
	public class Cryo<T> where T : IEquatable<T>
	{
		[SerializeField]
		private T _value;

		[SerializeField]
		public T value
		{
			get { return _value; }
			set
			{
				if (!value.Equals(_value) && onValueChanged != null)
					onValueChanged.Invoke();
				_value = value;
			}
		}
		public UnityAction onValueChanged;

        public Cryo(T value, UnityAction onValueChanged)
        {
            this.value = value;
            this.onValueChanged = onValueChanged;
        }

		public Cryo(T value)
		{
			this.value = value;
		}
	}

}