using System;

namespace DNA 
{
	public class WeakReference<T> : WeakReference 
	{
		/// <summary>
		/// The target reference.
		/// </summary>
		public new T Target 
		{
			get => 
				(T)base.Target;
			
			set => 
				this.Target = (object)value;
		}

		public WeakReference(object o) : base(o) {}
		
		public WeakReference(object o, bool trackResurection) 
			: base(o, trackResurection) {}
	}
}
