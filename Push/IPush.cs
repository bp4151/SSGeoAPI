using System;

namespace GeoAPI
{
	public interface IPush
	{
		string Notify (string channel, string to_ids, string payload, string filterType);
	}
}

