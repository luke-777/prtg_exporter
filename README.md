# prtg_exporter
A very simple example of how to make [PRTG Network Monitor](https://www.paessler.com/prtg) metrics available to [Prometheus](https://prometheus.io/).

prtg-exporter-core uses [PrtgAPI](https://github.com/lordmilko/PrtgAPI) to receive the metrics and
[prometheus-net](https://github.com/prometheus-net/prometheus-net) to export metrics to Prometheus.


![Grafana](https://raw.githubusercontent.com/luke-777/prtg_exporter/main/images/grafana.PNG)

![Prometheus](https://raw.githubusercontent.com/luke-777/prtg_exporter/main/images/prometheus.PNG)


## Build
You can build the project with dotnet. Clone the repo and execute:
```powershell
dotnet build
```

Create a configuration file (prtgexporter.json) or use environment variables containing your server credentials:
```json
{
	"PRTG": {
		"Server": "http://localhost",
		"Username": "yourapiuser",
		"Password": "yourpassword"
	},

	"Exporter": {
		"Port": "1234",
		"RefreshInterval": 120
	}
}
```
