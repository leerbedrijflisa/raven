namespace Lisa.Raven
{
	public enum TokenType
	{
		// The root document token, represents the entire document.
		Document,

		// Represents a doctype. Value in this is the content of the doctype. Will not have children.
		//  <!DOCTYPE html blah blah> -> {'Value': 'html blah blah'}
		Doctype,

		// Represents a single enclosed HTML element.
		//  <p class="blah">Test!</p> =>
		//	{
		//		'Value': 'p',
		//		'Children':
		//		[
		//			{'Type': 'OpenTag'},
		//			{'Type': 'ElementContent'},
		//			{'Type': 'CloseTag'}
		//		]
		//	}
		//
		//  <img alt="there is no image" /> => {'Value': 'img', 'Children':[{'Type': 'SelfCloseTag'}]}
		Element,
		OpenTag,
		CloseTag,
		ElementContent,
		SelfCloseTag,
		Text
	}
}
