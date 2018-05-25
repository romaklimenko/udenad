const progressTrend = {
    render: function (url, selector) {
        var svg = d3.select(selector),
            margin = { top: 20, right: 20, bottom: 30, left: 50 },
            width = +svg.attr("width") - margin.left - margin.right,
            height = +svg.attr("height") - margin.top - margin.bottom,
            g = svg.append("g")
                .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

        var x = d3.scaleTime()
            .rangeRound([0, width]);

        var y = d3.scaleLinear()
            .rangeRound([height, 0]);

        var area = d3.area()
            .curve(d3.curveMonotoneX)
            .x(function (d) { return x(d.date); })
            .y0(function (d) { return y(d.learned); })
            .y1(function (d) { return y(d.seen); });

        d3.json(url, function (error, data) {
            if (error) throw error;

            data.forEach(function (value) {
                value.date = new Date(value.date);

                var timeDiff = Math.abs(data[0].date.getTime() - value.date.getTime());
                var diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24));

                value.shift = diffDays;
            });

            // y = a + bx
            // y = intercept + slope * x

            // all = intercept + slope * x

            var seen_regression = linearRegression(
                data.map(function (d) { return d.seen; }),
                data.map(function (d) { return d.shift; }));


            var learned_data = data.filter(d => d.learned > 0);
            var learned_regression = linearRegression(
                learned_data.map(function (d) { return d.learned; }),
                learned_data.map(function (d) { return d.shift; }));

            //x.domain(d3.extent(data, function (d) { return d.date; }));
            // x.domain([data[0].date, new Date(data[0].date).setDate(data[0].date.getDate() + data[0].all / seen_regression.slope)]);
            x.domain([data[0].date, data[data.length - 1].date]);
            y.domain([0, d3.max(data, function (d) { return d.seen; })]);

            var diffDays = Math.ceil(Math.abs(x.domain()[0].getTime() - x.domain()[1].getTime()) / (1000 * 3600 * 24));

            // area path
            g.append("path")
                .datum(data)
                .attr("fill", "#E31836")
                .attr("d", area);

            g.append("line")
                .attr("stroke", "#000")
                .attr("stroke-dasharray", "5, 5")
                .attr("x1", 0)
                .attr("y1", y(seen_regression.intercept))
                .attr("x2", width)
                .attr("y2", y(seen_regression.intercept + seen_regression.slope * diffDays));

            g.append("line")
                .attr("stroke", "#000")
                .attr("stroke-dasharray", "5, 5")
                .attr("x1", 0)
                .attr("y1", y(learned_regression.intercept))
                .attr("x2", width)
                .attr("y2", y(learned_regression.intercept + learned_regression.slope * diffDays));

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

            g.append("text")
                .attr("fill", "#000")
                .attr("x", margin.left)
                .attr("y", margin.top)
                .attr("font-size", "9pt")
                .text("Seen cards trend: y = "
                + Math.round(seen_regression.intercept * 1000) / 1000 + " + "
                + Math.round(seen_regression.slope * 1000) / 1000 + "x; "
                + "r² = " + Math.round(seen_regression.r2 * 1000) / 1000);

            g.append("text")
                .attr("fill", "#000")
                .attr("x", margin.left)
                .attr("y", margin.top + 15)
                .attr("font-size", "9pt")
                .text("Expected date when there will be no unseen cards: "
                + new Date(new Date(data[0].date).setDate(data[0].date.getDate() + Math.round((data[data.length - 1].all - seen_regression.intercept) / seen_regression.slope))).toDateString());

            g.append("text")
                .attr("fill", "#000")
                .attr("x", margin.left)
                .attr("y", margin.top + 45)
                .attr("font-size", "9pt")
                .text(
                "Learned cards trend: y = "
                + Math.round(learned_regression.intercept * 1000) / 1000 + " + "
                + Math.round(learned_regression.slope * 1000) / 1000 + "x; "
                + "r² = " + Math.round(learned_regression.r2 * 1000) / 1000);

            g.append("text")
                .attr("fill", "#000")
                .attr("x", margin.left)
                .attr("y", margin.top + 60)
                .attr("font-size", "9pt")
                .text("Expected date when all cards will be learned: "
                + new Date(new Date(data[0].date).setDate(data[0].date.getDate() + Math.round((data[data.length - 1].all - learned_regression.intercept) / learned_regression.slope))).toDateString());

            g.append("text")
                .attr("fill", "#000")
                .attr("x", margin.left)
                .attr("y", margin.top + 90)
                .attr("font-size", "9pt")
                .text("All cards: " + data[data.length - 1].all);

            g.append("text")
                .attr("fill", "#000")
                .attr("x", margin.left)
                .attr("y", margin.top + 105)
                .attr("font-size", "9pt")
                .text("Seen cards: " + data[data.length - 1].seen);

            g.append("text")
                .attr("fill", "#000")
                .attr("x", margin.left)
                .attr("y", margin.top + 120)
                .attr("font-size", "9pt")
                .text("Learned cards: " + data[data.length - 1].learned);
        });
    }
};

function linearRegression(y, x) {
    var lr = {};
    var n = y.length;
    var sum_x = 0;
    var sum_y = 0;
    var sum_xy = 0;
    var sum_xx = 0;
    var sum_yy = 0;

    for (var i = 0; i < y.length; i++) {
        sum_x += x[i];
        sum_y += y[i];
        sum_xy += (x[i] * y[i]);
        sum_xx += (x[i] * x[i]);
        sum_yy += (y[i] * y[i]);
    }

    lr['slope'] = (n * sum_xy - sum_x * sum_y) / (n * sum_xx - sum_x * sum_x);
    lr['intercept'] = (sum_y - lr.slope * sum_x) / n;
    lr['r2'] = Math.pow((n * sum_xy - sum_x * sum_y) / Math.sqrt((n * sum_xx - sum_x * sum_x) * (n * sum_yy - sum_y * sum_y)), 2);

    return lr;
}
