resource "aws_lambda_event_source_mapping" "stream_trigger" {
  event_source_arn  = aws_dynamodb_table.batman_table.stream_arn
  function_name     = aws_lambda_function.stream_processor.arn
  starting_position = "LATEST"

  batch_size = 100
  maximum_batching_window_in_seconds = 5
  maximum_retry_attempts = 3
  bisect_batch_on_function_error = true
  
  # CRITICAL: Enable partial batch failure reporting
  function_response_types = ["ReportBatchItemFailures"]
}