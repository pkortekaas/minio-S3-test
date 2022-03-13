# minio-S3-test
Some sample code using Amazon S3 buckets with the community edition of [MinIO](https://docs.min.io/).
This Object Storage engine is API compatible with Amazon S3 Cloud storage service, and thereby ideal for local software development.

### Requirements
- .NET 6.0

### Usage
- Get a copy of this example using: `git clone https://github.com/pkortekaas/minio-S3-test.git`
- Get a copy of [MinIO](https://docs.min.io/)
- Start MinIO server `minio server --console-address ":9001" /<data location>`
````
API: http://192.168.1.1:9000  http://127.0.0.1:9000
RootUser: minioadmin
RootPass: minioadmin

Console: http://192.168.1.1:9001 http://127.0.0.1:9001
RootUser: minioadmin
RootPass: minioadmin
````
- Use a browser to open the Console at http://127.0.0.1:9001
- Create the Bucket to be used
- Create a User
- Select this User, create a Service Account and make sure you have a readwrite Policy
- Check the Settings->Configurations for the proper Server Location, e.g. eu-west-1
- copy sample.settings.json to setting.json and check the required MinIO settings
