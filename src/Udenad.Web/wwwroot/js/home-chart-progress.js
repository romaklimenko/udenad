var charts = charts || {};

charts.progress = {
    render: function (url, selector) {
        var svg = d3.select(selector),
            margin = { top: 20, right: 20, bottom: 30, left: 50 },
            width = +svg.attr("width") - margin.left - margin.right,
            height = +svg.attr("height") - margin.top - margin.bottom,
            g = svg.append("g").attr("transform", "translate(" + margin.left + "," + margin.top + ")");

        var x = d3.scaleTime()
            .rangeRound([0, width]);

        var y = d3.scaleLinear()
            .rangeRound([height, 0]);

        var area = d3.area()
            .x(function (d) { return x(d.date); })
            .y0(function (d) { return y(d.mature); })
            .y1(function (d) { return y(d.seen); });

        d3.json(url, function (error, data) {
            if (error) throw error;

            data.forEach(function (value) {
                value.date = new Date(value.date);
            });

            console.log(charts.progress.regression(data));

            x.domain(d3.extent(data, function (d) { return d.date; }));
            y.domain([0, d3.max(data, function (d) { return d.all; })]);

            // area path
            g.append("path")
                .datum(data)
                .attr("fill", "red")
                .attr("d", area);

            // bottom axis (x)
            g.append("g")
                .attr("transform", "translate(0," + height + ")")
                .call(d3.axisBottom(x));

            // left axis (y)
            g.append("g")
                .call(d3.axisLeft(y))
                .append("text")
                .attr("fill", "#000")
                .attr("transform", "rotate(-90)")
                .attr("y", 6)
                .attr("dy", "0.71em")
                .attr("text-anchor", "end");
        });
    },

    regression: function (data) {
        return "regression";
    }
};
