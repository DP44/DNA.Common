using System;

namespace DNA 
{
	public interface IFilter<T> 
	{
		bool Filter(T t);
	}
}
