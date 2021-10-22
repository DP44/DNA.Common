using System;
using DNA.Drawing;
using DNA.Net.GamerServices;

namespace DNA.Avatars.Actions 
{
	public class SetExpressionAction : State 
	{
		private AvatarExpression _expression;
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public SetExpressionAction(AvatarExpression expression) => 
			this._expression = expression;
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected override void OnStart(Entity entity) 
		{
			((Avatar)entity).Expression = this._expression;
			
			base.OnStart(entity);
		}
	}
}
