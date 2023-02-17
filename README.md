# scotec Software Solutions AB

Today, the development of new applications is hardly conceivable without open source software. The use of ready-made software components reduces development time and lowers costs.

We also rely on open source in our development and would like to give something back to the community. We have therefore decided to publish reusable code from our projects.

Our libraries are constantly updated and improved. The individual components are initially tailored to the use-cases in our projects. Suggestions for improvements or extensions are welcome.

Feel free to visit us on our [website](https://www.scotec-software.com).


## Our components

### Scotec.AmazonS3.Streaming
For the upload as well as for the MultiPartUpload, the AmazonS3Client needs the total size of the content to be transferred before the upload begins. Often, however, the size is unknown. This can be the case, for example, when the content to be transferred is generated. So that the upload can take place, the entire content must first be available either in the memory or on a storage medium. With large amounts of data, this quickly leads to a load on the main memory or to performance losses due to hard disk accesses.

The scotec-AmazonS3WriteStream is able to perform a multipart upload without knowing the size of the content beforehand. However, this means that the multipart upload cannot be completed automatically. Therefore, Flush() must be called after all data has been passed to the stream. Disposing the stream without first calling Flush() will abort the upload. All parts transferred to the bucket so far are deleted in this case.

### Scotec.Web.Robots
Generates dynamic robots.txt and sitemap.xml files for Blazor websites.

### Scotec.Blazor.Markdown
This Blazor component can be used to display Markdown content in Blazor apps.
The component is built on top of the [Markdig](https://github.com/xoofx/markdig) parser.

### Scotec.Extensions.Linq
...

### Scotec.Math
...

### Scotec.Math.Units
...

### Scotec.Events.WeakEvents
...

### Scotec.Queues
...

### Scotec.RingBuffer
...

### Scotec.Smtp.Service
...

### Scotec.XmlDatabase
...

### Scotec.Transactions
...