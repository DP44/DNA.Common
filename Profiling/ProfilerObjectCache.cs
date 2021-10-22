using System;

namespace DNA.Profiling
{
	public class ProfilerObjectCache<iType> 
		where iType : class, IProfilerLinkedListNode, new()
	{
		private int _growSize = 5;
		
		private ProfilerLockFreeStack<iType> _cache = 
			new ProfilerLockFreeStack<iType>();

		public int GrowSize
		{
			get => 
				this._growSize;
			
			set => 
				this._growSize = value > 0 
					? value 
					: throw new ArgumentException(
						"PartCache.GrowSize must be a positive integer");
		}

		private void GrowList(int size)
		{
			for (int i = 0; i < size; i++)
			{
				this._cache.Push(Activator.CreateInstance<iType>());
			}
		}

		public iType Get()
		{
			iType type = default(iType);
			type = this._cache.Pop();
			
			if (type == null)
			{
				this.GrowList(this._growSize);

				for (type = this._cache.Pop(); type == null; type = this._cache.Pop())
				{
					this._cache.Push(Activator.CreateInstance<iType>());
				}
			}
			
			return iType;
		}

		public void Put(iType part) => 
			this._cache.Push(part);

		public void PutList(iType list) => 
			this._cache.PushList(list);
	}
}
