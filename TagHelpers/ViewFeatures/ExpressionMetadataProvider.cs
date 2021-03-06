﻿using System;
using System.Globalization;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace CRM.TagHelpers.ViewFeatures
{
	class ExpressionMetadataProvider
	{/// <summary>
	 /// Gets <see cref="ModelExplorer"/> for named <paramref name="expression"/> in given
	 /// <paramref name="viewData"/>.
	 /// </summary>
	 /// <param name="expression">Expression name, relative to <c>viewData.Model</c>.</param>
	 /// <param name="viewData">
	 /// The <see cref="ViewDataDictionary"/> that may contain the <paramref name="expression"/> value.
	 /// </param>
	 /// <param name="metadataProvider">The <see cref="IModelMetadataProvider"/>.</param>
	 /// <returns>
	 /// <see cref="ModelExplorer"/> for named <paramref name="expression"/> in given <paramref name="viewData"/>.
	 /// </returns>
		public static ModelExplorer FromStringExpression(
			string expression,
			ViewDataDictionary viewData,
			IModelMetadataProvider metadataProvider)
		{
			ViewDataInfo viewDataInfo = ViewDataEvaluator.Eval(viewData, expression);

			if (viewDataInfo is null)
			{
				// Try getting a property from ModelMetadata if we couldn't find an answer in ViewData
				ModelExplorer propertyExplorer = viewData.ModelExplorer.GetExplorerForProperty(expression);

				if (propertyExplorer is not null)
				{
					return propertyExplorer;
				}
			}

			if (viewDataInfo is not null)
			{
				if (viewDataInfo.Container == viewData
					&& viewDataInfo.Value == viewData.Model
					&& string.IsNullOrEmpty(expression))
				{
					// Nothing for empty expression in ViewData and ViewDataEvaluator just returned the model. Handle
					// using FromModel() for its object special case.
					return FromModel(viewData, metadataProvider);
				}

				ModelExplorer containerExplorer = viewData.ModelExplorer;

				if (viewDataInfo.Container is not null)
				{
					containerExplorer = metadataProvider.GetModelExplorerForType(
						viewDataInfo.Container.GetType(),
						viewDataInfo.Container);
				}

				if (viewDataInfo.PropertyInfo is not null)
				{
					// We've identified a property access, which provides us with accurate metadata.
					ModelMetadata containerMetadata = metadataProvider.GetMetadataForType(viewDataInfo.Container?.GetType());
					ModelMetadata propertyMetadata = containerMetadata.Properties[viewDataInfo.PropertyInfo.Name];

					object modelAccessor(object ignore)
					{
						return viewDataInfo.Value;
					}

					return containerExplorer.GetExplorerForExpression(propertyMetadata, modelAccessor);
				}
				else if (viewDataInfo.Value is not null)
				{
					// We have a value, even though we may not know where it came from.
					ModelMetadata valueMetadata = metadataProvider.GetMetadataForType(viewDataInfo.Value.GetType());
					return containerExplorer.GetExplorerForExpression(valueMetadata, viewDataInfo.Value);
				}
			}

			// Treat the expression as string if we don't find anything better.
			ModelMetadata stringMetadata = metadataProvider.GetMetadataForType(typeof(string));
			return viewData.ModelExplorer.GetExplorerForExpression(stringMetadata, modelAccessor: null);
		}

		private static ModelExplorer FromModel(ViewDataDictionary viewData, IModelMetadataProvider metadataProvider)
		{
			if (viewData.ModelMetadata.ModelType == typeof(object))
			{
				// Use common simple type rather than object so e.g. Editor() at least generates a TextBox.
				var model = viewData.Model is null ? null : Convert.ToString(viewData.Model, CultureInfo.CurrentCulture);
				return metadataProvider.GetModelExplorerForType(typeof(string), model);
			}

			else
			{
				return viewData.ModelExplorer;
			}
		}
	}
}