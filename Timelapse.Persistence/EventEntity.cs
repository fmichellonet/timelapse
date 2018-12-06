﻿namespace Timelapse.Persistence
{
	using Microsoft.WindowsAzure.Storage.Table;

	public class EventEntity : TableEntity
	{
		public string Type { get; set; }
		public string Data { get; set; }
	}
}