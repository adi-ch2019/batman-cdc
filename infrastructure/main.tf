terraform {
  required_version = ">= 1.5.0"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = ">= 6.0.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "~> 3.0"
    }
  }
}

variable "aws_region" {
  default = "us-east-1"
}

variable "aws_profile" {
  description = "AWS CLI profile used for deployment"
  default     = "default"
}

variable "api_artifact_key" {
  default = "api/latest.zip"
}

variable "stream_artifact_key" {
  default = "stream/latest.zip"
}

provider "aws" {
  region  = var.aws_region
  profile = var.aws_profile
}