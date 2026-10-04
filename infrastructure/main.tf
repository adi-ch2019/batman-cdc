terraform {
  required_version = ">= 1.5.0"
  
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }
  }

  # Optional: Remote state backend for team collaboration
  # backend "s3" {
  #   bucket = "your-terraform-state-bucket"
  #   key    = "batman-cdc/terraform.tfstate"
  #   region = "us-east-1"
  # }
}

provider "aws" {
  region = var.aws_region
}

variable "aws_region" {
  default = "us-east-1"
}

variable "api_artifact_key" {
  description = "S3 key for API Lambda artifact"
  default     = "api/latest.zip"
}

variable "stream_artifact_key" {
  description = "S3 key for Stream Processor artifact"
  default     = "stream/latest.zip"
}