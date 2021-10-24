﻿using AzureTablesLifecycleManager.AzureDAL.Models;
using System;
using Xunit;

namespace AzureTablesLifecycleManager.BRL.Tests.IntegrationTests.Services
{
	public class QueryBuilderTests
	{
		public QueryBuilderTests()
		{

		}


		[Fact]
		public void AppendCondition_OneValidCondition_PassesOK()
		{
			var factory = new QueryBuilder();
			var tableName = "ATableIWantToGrab";
			string condition = ODataPredefinedFilters.TableNameExact(tableName);

			// Act
			var result = factory.AppendCondition(
				condition).Build();

			// Assert
			Assert.Equal($"TableName eq '{tableName}'", result);
		}

		[Fact]
		public void AppendCondition_TwoValidConditionalsWithAndOperator_PassesOK()
		{
			var factory = new QueryBuilder();
			var rowKey = Guid.NewGuid().ToString();
			var partitionKey = Guid.NewGuid().ToString();
			string condition1 = ODataPredefinedFilters.RowKeyExact(rowKey);
			string condition2 = ODataPredefinedFilters.PartitionKeyExact(partitionKey);

			// Act
			var result = factory
				.AppendCondition(condition1)
				.And()
				.AppendCondition(condition2)
				.Build();

			// Assert
			Assert.Equal($"RowKey eq '{rowKey}' and PartitionKey eq '{partitionKey}'", result);

		}

		[Fact]
		public void QueryBuilder_MultipleConditionsWithCorrectConfig_PassesOK()
		{
			var factory = new QueryBuilder();
			var rowKey = new Guid("512ef724-17dc-44a9-8e32-93fc212dbb4a").ToString();
			var partitionKey = new Guid("f1142899-b6e4-4e0a-aecb-58fdc23df10f").ToString();
			var date1 = DateTime.Parse("2021-10-21T20:42:03.2034035+01:00");
			var date2 = DateTime.Parse("2021-10-24T20:42:03.2039446+01:00");
			string condition1 = ODataPredefinedFilters.RowKeyExact(rowKey);
			string condition2 = ODataPredefinedFilters.PartitionKeyExact(partitionKey);
			string condition3 = ODataPredefinedFilters.TimestampGreaterThan(date1);
			string condition4 = ODataPredefinedFilters.TimestampLessThan(date2);

			// Act
			var result = factory
				.AppendCondition(condition1)
				.And()
				.StartSubCondition()
					.AppendCondition(condition2)
					.Or()
					.AppendCondition(condition3)
				.EndSubCondition()
				.And()
				.AppendCondition(condition4)
				.Build();

			// Assert
			Assert.Equal($"RowKey eq '{rowKey}' and" +
				$" (PartitionKey eq '{partitionKey}' or " +
				$"Timestamp gt '{date1.ToString("o")}') and " +
				$"Timestamp lt '{date2.ToString("o")}'", result);

		}
	}
}
