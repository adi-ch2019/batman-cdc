resource "aws_dynamodb_table" "batman_table" {
  name         = "batman-cdc-table"
  billing_mode = "PAY_PER_REQUEST"
  hash_key     = "Id"

  attribute {
    name = "Id"
    type = "S"
  }

  # CRITICAL: Enable Streams for CDC
  stream_enabled   = true
  stream_view_type = "NEW_AND_OLD_IMAGES"

  tags = {
    Project     = "BATMAN-CDC"
    Environment = "portfolio"
  }
}