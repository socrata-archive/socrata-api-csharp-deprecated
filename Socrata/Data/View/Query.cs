using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Text;

namespace Socrata.Data.View
{
	public class Query
	{
		public IList<OrderBy> orderBys;
		public ExpressionNode filterCondition;
	}
	
	public class OrderBy
	{
		public Boolean ascending;
		public ExpressionNode expression;
	}
	
	public class ExpressionNode
	{
		public enum ExpressionType
		{
			LITERAL,
			OPERATOR,
			COLUMN
		};
		
		public string type { get; private set; }
		
		public int columnId;
		public string value;
		public string columnFieldName;
		public List<ExpressionNode> children;
		
		public ExpressionType Type()
		{
			return (ExpressionType) Enum.Parse(typeof(ExpressionType), type.ToUpper());
		}
		
		private string expressionValue()
		{
			switch(Type())
			{
			case ExpressionType.COLUMN:
				return String.Format ("Column {0}", columnFieldName != null ? columnFieldName : columnId.ToString());
			case ExpressionType.OPERATOR:
				return value;
			case ExpressionType.LITERAL:
				return String.Format ("\"{0}\"", value);
			default:
				return "(no expression)";
			}
		}
		
		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Socrata.Data.View.ExpressionNode"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents the current <see cref="Socrata.Data.View.ExpressionNode"/>.
		/// </returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder(expressionValue());
			
			if (children != null && children.Count > 0)
			{
				sb.Append (": ");
				bool first = true;
				if (children.Count > 0)
					sb.Append ("(");
					
				foreach(ExpressionNode child in children)
				{
					if (first)
						first = false;
					else
						sb.Append (", ");
					
					sb.Append(child.ToString());
				}
				
				
				if (children.Count > 0)
					sb.Append (")");
			}
			return sb.ToString();
		}
	}
}