(function forecast() {
    var svg = d3.select("#chart-forecast"),
        margin = {top: 20, right: 20, bottom: 30, left: 50},
        width = +svg.attr("width") - margin.left - margin.right,
        height = +svg.attr("height") - margin.top - margin.bottom,
        g = svg.append("g").attr("transform", "translate(" + margin.left + "," + margin.top + ")");

    var x = d3.scaleTime()
        .rangeRound([0, width]);

    var y = d3.scaleLinear()
        .rangeRound([height, 0]);

    var area = d3.area()
        .x(function(d) { return x(d.date); })
        .y0(function(d) { return y(0); })
        .y1(function(d) { return y(d.count); })
        .curve(d3.curveCatmullRom.alpha(0.5));

    d3.json("forecast", function(error, data) {
        if (error) throw error;

        data.forEach(function(d) {
            d.date = new Date(d.date).setHours(0,0,0,0);
        });

        if (d3.min(data, function (d) { return d.date }) > new Date().setHours(0,0,0,0)) {
            data = [{date: new Date().setHours(0,0,0,0), count: 0 }].concat(data);
        }
        
        x.domain(d3.extent(data, function(d) { return d.date; }));
        y.domain([0, d3.max(data, function(d) { return d.count; })]);

        g.append("path")
            .datum(data)
            .attr("fill", "#E31836")
            .attr("d", area);

        g.append("g")
            .attr("transform", "translate(0," + height + ")")
            .call(d3.axisBottom(x));

        g.append("g")
            .call(d3.axisLeft(y))
            .append("text")
            .attr("fill", "#000")
            .attr("transform", "rotate(-90)")
            .attr("y", 6)
            .attr("dy", "0.71em")
            .attr("text-anchor", "end");
    });    
})();

