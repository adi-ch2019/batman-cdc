output "api_endpoint" {
  value = aws_apigatewayv2_stage.default.invoke_url
}

output "dynamodb_table" {
  value = aws_dynamodb_table.batman_table.name
}

output "artifacts_bucket" {
  value = aws_s3_bucket.artifacts.bucket
}