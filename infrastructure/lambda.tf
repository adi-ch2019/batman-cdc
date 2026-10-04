resource "aws_lambda_function" "api_lambda" {
  function_name = "batman-api-handler"
  role          = aws_iam_role.api_lambda_role.arn
  # ⚠️ CHANGE: Use provided.al2023, NOT dotnet10
  runtime       = "provided.al2023" 
  # ⚠️ CHANGE: Handler is "bootstrap" for custom runtime
  handler       = "bootstrap"       
  timeout       = 30
  memory_size   = 256

  s3_bucket = aws_s3_bucket.artifacts.bucket
  s3_key    = var.api_artifact_key

  environment {
    variables = {
      TABLE_NAME = aws_dynamodb_table.batman_table.name
    }
  }

  publish = true
}

resource "aws_lambda_function" "stream_processor" {
  function_name = "batman-stream-processor"
  role          = aws_iam_role.stream_lambda_role.arn
  # ⚠️ CHANGE: Use provided.al2023
  runtime       = "provided.al2023"
  # ⚠️ CHANGE: Handler is "bootstrap"
  handler       = "bootstrap"
  timeout       = 60
  memory_size   = 512

  s3_bucket = aws_s3_bucket.artifacts.bucket
  s3_key    = var.stream_artifact_key

  publish = true
}