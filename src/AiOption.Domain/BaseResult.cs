﻿using EventFlow.Aggregates.ExecutionResults;

namespace AiOption.Domain {

    public class BaseResult : IExecutionResult {


        public BaseResult(bool isSuccess, string message = null) {
            IsSuccess = isSuccess;
            Message = message;
        }

        public string Message { get; protected set; }


        public bool IsSuccess { get; }

    }

}