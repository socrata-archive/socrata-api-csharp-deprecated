namespace Socrata
{
	public class Strings
	{
		public static string TitleCase(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return string.Empty;
			}
			char[] chars = input.ToCharArray();
			chars[0] = char.ToUpper(chars[0]);
			return new string(chars);
		}
	}
}
