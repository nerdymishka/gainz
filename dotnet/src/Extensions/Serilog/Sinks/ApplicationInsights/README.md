
The application Insights code is based on/forked from the Serilog sink found here on github:
https://github.com/serilog/serilog-sinks-applicationinsights

It's still licensed under Apache 2.0

The main difference is that this one using an ITelemetryClient interface for DI purposes an swapping
out implementation as needed.  
