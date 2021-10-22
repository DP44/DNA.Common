using System;

namespace DNA.Distribution
{
	public class NullOnlineServices : OnlineServices
	{
		public NullOnlineServices(Guid productID) : base(productID)
		{
			this._username = "Player";
		}

		public override DateTime GetServerTime()
		{
			return DateTime.UtcNow;
		}

		public override bool ValidateLicense(string userName, string password, out string reason)
		{
			this._username = userName;
			reason = "success";
			return true;
		}

		public override bool ValidateLicenseFacebook(string facebookID, string accessToken, out string username, out string reason)
		{
			reason = "facebookUser";
			username = "facebookUser";
			this._username = reason;
			return true;
		}

		public override void AcceptTerms(string userName, string password)
		{
		}

		public override void AcceptTermsFacebook(string facebookID)
		{
		}

		public override bool RegisterFacebook(string facebookID, string accessToken, string email, string userName, string password, out string reason)
		{
			reason = "success";
			return true;
		}

		public override string GetLauncherPage()
		{
			return "http://www.castleminer.com";
		}

		public override string GetProductTitle()
		{
			return "Null Title";
		}

		public override int? GetAddOn(Guid guid)
		{
			return null;
		}

		public override void Update(TimeSpan elapsedTime, TimeSpan totalTime)
		{
		}
	}
}
