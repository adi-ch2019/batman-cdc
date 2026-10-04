resource "random_id" "bucket_suffix" {
  byte_length = 4
}

resource "aws_s3_bucket" "artifacts" {
  bucket = "batman-lambda-artifacts-${random_id.bucket_suffix.hex}"

  tags = {
    Project = "BATMAN-CDC"
  }
}