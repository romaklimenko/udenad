(function forecast() {
    var margin = {top: 20, right: 20, bottom: 70, left: 50},
        width = 960 - margin.left - margin.right,
        height = 500 - margin.top - margin.bottom;

    var x = d3.scaleBand().rangeRound([0, width], .05).padding(0.1);
    var y = d3.scaleLinear().range([height, 0]);
    var xAxis = d3.axisBottom().scale(x);
    var yAxis = d3.axisLeft()
        .scale(y)
        .ticks(10);
    var svg = d3.select("#chart-repetitions")
        .attr("width", width + margin.left + margin.right)
        .attr("height", height + margin.top + margin.bottom)
        .append("g")
        .attr("transform",
            "translate(" + margin.left + "," + margin.top + ")");
    d3.json("repetitions", function(error, data) {
        x.domain(data.map(function(d) { return d.repetitions; }));
        y.domain([0, d3.max(data, function(d) { return d.count; })]);
        svg.append("g")
            .attr("class", "x axis")
            .attr("transform", "translate(0," + height + ")")
            .call(xAxis)
            .selectAll("text")
            .style("text-anchor", "end")
            .attr("dx", "-.8em")
            .attr("dy", "-.55em")
            .attr("transform", "rotate(-90)" );
        svg.append("g")
            .attr("class", "y axis")
            .call(yAxis)
            .append("text");
        svg.selectAll("bar")
            .data(data)
            .enter().append("rect")
            .style("fill", "#E31836")
            .attr("x", function(d) { return x(d.repetitions); })
            .attr("width", x.bandwidth())
            .attr("y", function(d) { return y(d.count); })
            .attr("height", function(d) { return height - y(d.count); });
    });
})();

