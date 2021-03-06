﻿using CRM.TagHelpers.Enums;
using CRM.TagHelpers.TagHelpers.Fast;
using CRM.TagHelpers.ViewFeatures;

using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CRM.TagHelpers.TagHelpers.Fluent
{
	/// <summary>
	/// <see cref="FastTextFieldTagHelper"/> implementation targeting &lt;fluent-text-field&gt;
	/// elements with an <c>asp-for</c> attribute.
	/// </summary>
	[HtmlTargetElement("fluent-text-field", Attributes = ForAttributeName, TagStructure = TagStructure.NormalOrSelfClosing)]
	public class FluentTextFieldTagHelper : FastTextFieldTagHelper
	{
		/// <summary>
		/// Creates a new instance of <see cref="FluentTextFieldTagHelper"/>.
		/// </summary>
		/// <param name="generator">The <see cref="FastGenerator"/>.</param>
		public FluentTextFieldTagHelper(IHtmlGenerator generator) : base(generator)
		{
			DesignSystem = DesignSystem.Fluent;
			Appearance = TextFieldAppearance.Filled;
		}
	}
}