var charts = charts || {};

charts.forecast = {
    render: function (url, selector) {
        var margin = { top: 20, right: 20, bottom: 80, left: 50 },
            width = 1200 - margin.left - margin.right,
            height = 500 - margin.top - margin.bottom;

        var x = d3.scaleBand().rangeRound([0, width], .05).padding(0.1);
        var y = d3.scaleLinear().range([height, 0]);
        var xAxis = d3.axisBottom()
            .scale(x)
            .tickFormat(d3.timeFormat("%Y-%m-%d %a"));
        var yAxis = d3.axisLeft()
            .scale(y)
            .ticks(10);
        var svg = d3.select("#chart-forecast")
            .attr("width", width + margin.left + margin.right)
            .attr("height", height + margin.top + margin.bottom)
            .append("g")
            .attr("transform",
            "translate(" + margin.left + "," + margin.top + ")");
        d3.json(url, function (error, data) {
            data.forEach(function (d) {
                var date = new Date(d.date);
                date.setHours(0, 0, 0, 0)
                d.date = date;
            });

            var today = new Date().setHours(0, 0, 0, 0);

            if (d3.min(data, function (d) { return d.date }) > today) {
                data = [{ date: today, count: 0, average: 0 }].concat(data);
            }

            var extent = d3.extent(data.map(function (d) { return d.date; }));

            var range = d3.timeDays(extent[0], new Date(extent[1]).setDate(extent[1].getDate() + 1));

            data = range.map(function (d) {
                for (var index = 0; index < data.length; index++) {
                    var element = data[index];
                    if (element.date.toString() == d.toString()) {
                        return element;
                    }
                }
                return {
                    date: d,
                    count: 0,
                    average: 0
                };
            });

            var maxAverage = d3.max(data, function(d) { return d.average } );

            x.domain(data.map(function (d) { return d.date; }));
            y.domain([0, d3.max(data, function (d) { return d.count; })]);
            svg.append("g")
                .attr("class", "x axis")
                .attr("transform", "translate(0," + height + ")")
                .call(xAxis)
                .selectAll("text")
                .style("text-anchor", "end")
                .attr("font-size", "8")
                .attr("dx", "-.8em")
                .attr("dy", "-.55em")
                .attr("transform", "rotate(-90)");
            svg.append("g")
                .attr("class", "y axis")
                .call(yAxis)
                .append("text");
            svg.selectAll("bar")
                .data(data)
                .enter().append("rect")
                .style("fill", "#E31836")
                .style("opacity", function (d) { return d.average / maxAverage; })
                .attr("x", function (d) { return x(d.date); })
                .attr("width", function(d) { return x.bandwidth() * d.average / maxAverage; })
                .attr("y", function (d) { return y(d.count); })
                .attr("height", function (d) { return height - y(d.count); })
                .append("title").text(function(d) { return Math.round(d.average); });
            svg.selectAll("bar")
                .data(data)
                .enter().append("text")
                .attr("x", function (d) { return x(d.date) + x.bandwidth() / 2; })
                .attr("y", function (d) { return y(d.count) - 5; })
                .attr("text-anchor", "middle")
                .attr("width", x.bandwidth())
                .attr("font-size", "8")
                .text(function (d) { return d.count; });
        });
    }
};
