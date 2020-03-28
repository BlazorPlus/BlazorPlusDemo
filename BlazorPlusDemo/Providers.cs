using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorPlus;

namespace BlazorPlusDemo
{
	public class WebCustomizeSession : BlazorSession
	{
		public WebCustomizeSession(Microsoft.JSInterop.IJSRuntime jsr, Microsoft.AspNetCore.Http.IHttpContextAccessor hca) : base(jsr, hca)
		{
		}

		public override Type TypeGetUIDialogAlert(UIDialogOption option)
		{
			return typeof(CustomizeUI.UIDialogAlert);
		}
		public override Type TypeGetUIDialogConfirm(UIDialogOption option)
		{
			return typeof(CustomizeUI.UIDialogConfirm);
		}
		public override Type TypeGetUIDialogPrompt(UIDialogOption option)
		{
			return typeof(CustomizeUI.UIDialogPrompt);
		}
	}
}
