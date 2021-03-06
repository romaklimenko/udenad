const repetitions = {
    render: function (url, selector) {
        var margin = { top: 20, right: 20, bottom: 70, left: 50 },
            width = 1200 - margin.left - margin.right,
            height = 600 - margin.top - margin.bottom;

        var x = d3.scaleBand().rangeRound([0, width], .05).padding(0.1);
        var y = d3.scaleLinear().range([height, 0]);
        var xAxis = d3.axisBottom().scale(x);
        var yAxis = d3.axisLeft()
            .scale(y)
            .ticks(10);
        var svg = d3.select(selector)
            .attr("width", width + margin.left + margin.right)
            .attr("height", height + margin.top + margin.bottom)
            .append("g")
            .attr("transform",
            "translate(" + margin.left + "," + margin.top + ")");
        d3.json(url, function (error, data) {
            x.domain(data.map(function (d) { return d.repetitions; }));
            y.domain([0, d3.max(data, d => d.count)]);
            svg.append("g")
                .attr("class", "x axis")
                .attr("transform", "translate(0," + height + ")")
                .call(xAxis)
                .selectAll("text")
                .style("text-anchor", "middle")
                //.attr("dx", "-.8em")
                //.attr("dy", "-.55em")
                //.attr("transform", "rotate(-90)")
                .text((d) => d == '11' ? 'Learned' : d);
            svg.append("g")
                .attr("class", "y axis")
                .call(yAxis)
                .append("text");
            svg.selectAll("bar")
                .data(data)
                .enter().append("rect")
                .style("fill", "#D50000")
                .attr("x", function (d) { return x(d.repetitions); })
                .attr("width", x.bandwidth())
                .attr("y", function (d) { return y(d.count); })
                .attr("height", function (d) { return height - y(d.count); })

            svg.selectAll("bar")
                .data(data)
                .enter().append("text")
                .attr("x", function (d) { return x(d.repetitions) + x.bandwidth() / 2; })
                .attr("y", function (d) { return y(d.count) - 5; })
                .attr("text-anchor", "middle")
                .attr("width", x.bandwidth())
                .attr("font-size", "12")
                .text(function (d) { return d.count; });
        });
    }
};
