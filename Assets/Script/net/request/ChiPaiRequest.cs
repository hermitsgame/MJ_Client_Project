﻿using System;
using LitJson;

namespace AssemblyCSharp
{
	public class ChiPaiRequest: ClientRequest
	{
		public ChiPaiRequest (CardVO cardVO)
		{
			headCode = APIS.CHIPAI_REQUEST;
			msg = JsonMapper.ToJson (cardVO);
		}
	}
}

