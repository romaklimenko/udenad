const progressTrend = {
    render: (url, selector) => {
        const svg = d3.select(selector)
        const margin = { top: 20, right: 20, bottom: 30, left: 50 }
        const width = +svg.attr('width') - margin.left - margin.right
        const height = +svg.attr('height') - margin.top - margin.bottom
        const g = svg
            .append('g')
            .attr('transform', `translate(${margin.left},${margin.top})`)

        const x = d3.scaleTime().rangeRound([0, width])

        const y = d3.scaleLinear().rangeRound([height, 0])

        const area = d3.area()
            .curve(d3.curveMonotoneX)
            .x(d => x(d.date))
            .y0(d => y(d.learned))
            .y1(d => y(d.seen))

        d3.json(url, (error, data) => {
            if (error) throw error

            data.forEach(value => {
                value.date = new Date(value.date)

                const timeDiff = Math.abs(data[0].date.getTime() - value.date.getTime())
                const diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24))

                value.shift = diffDays
            })

            // y = a + bx
            // y = intercept + slope * x

            // all = intercept + slope * x

            const seen_regression = linearRegression(
                data.map(d => d.seen),
                data.map(d => d.shift))

            const learned_data = data.filter(d => d.learned > 0)
            const learned_regression = linearRegression(
                learned_data.map(d => d.learned),
                learned_data.map(d => d.shift))

            x.domain([data[0].date, data[data.length - 1].date])
            y.domain([0, d3.max(data, d => d.seen)])

            const diffDays = Math.ceil(
                Math.abs(x.domain()[0].getTime() - x.domain()[1].getTime()) / (1000 * 3600 * 24))

            // area path
            g.append('path')
                .datum(data)
                .attr('fill', '#D50000')
                .attr('d', area)

            g.append('line')
                .attr('stroke', '#000')
                .attr('stroke-dasharray', '5, 5')
                .attr('x1', 0)
                .attr('y1', y(seen_regression.intercept))
                .attr('x2', width)
                .attr('y2', y(seen_regression.intercept + seen_regression.slope * diffDays))

            g.append('line')
                .attr('stroke', '#000')
                .attr('stroke-dasharray', '5, 5')
                .attr('x1', 0)
                .attr('y1', y(learned_regression.intercept))
                .attr('x2', width)
                .attr('y2', y(learned_regression.intercept + learned_regression.slope * diffDays))

            // bottom axis (x)
            g.append('g')
                .attr('transform', `translate(0,${height})`)
                .call(d3.axisBottom(x))

            // left axis (y)
            g.append('g')
                .call(d3.axisLeft(y))
                .append('text')
                .attr('fill', '#000')
                .attr('transform', 'rotate(-90)')
                .attr('y', 6)
                .attr('dy', '0.71em')
                .attr('text-anchor', 'end')

            g.append('text')
                .attr('fill', '#000')
                .attr('x', margin.left)
                .attr('y', margin.top)
                .attr('font-size', '9pt')
                .text('Seen cards trend: y = '
                + Math.round(seen_regression.intercept * 1000) / 1000 + ' + '
                + Math.round(seen_regression.slope * 1000) / 1000 + 'x; '
                + 'r² = ' + Math.round(seen_regression.r2 * 1000) / 1000)

            g.append('text')
                .attr('fill', '#000')
                .attr('x', margin.left)
                .attr('y', margin.top + 15)
                .attr('font-size', '9pt')
                .text('Expected date when there will be no unseen cards: '
                + new Date(new Date(data[0].date).setDate(data[0].date.getDate() + Math.round((data[data.length - 1].all - seen_regression.intercept) / seen_regression.slope))).toDateString())

            g.append('text')
                .attr('fill', '#000')
                .attr('x', margin.left)
                .attr('y', margin.top + 45)
                .attr('font-size', '9pt')
                .text(
                'Learned cards trend: y = '
                + Math.round(learned_regression.intercept * 1000) / 1000 + ' + '
                + Math.round(learned_regression.slope * 1000) / 1000 + 'x; '
                + 'r² = ' + Math.round(learned_regression.r2 * 1000) / 1000)

            g.append('text')
                .attr('fill', '#000')
                .attr('x', margin.left)
                .attr('y', margin.top + 60)
                .attr('font-size', '9pt')
                .text('Expected date when all cards will be learned: '
                + new Date(new Date(data[0].date).setDate(data[0].date.getDate() + Math.round((data[data.length - 1].all - learned_regression.intercept) / learned_regression.slope))).toDateString())

            g.append('text')
                .attr('fill', '#000')
                .attr('x', margin.left)
                .attr('y', margin.top + 90)
                .attr('font-size', '9pt')
                .text('All cards: ' + data[data.length - 1].all)

            g.append('text')
                .attr('fill', '#000')
                .attr('x', margin.left)
                .attr('y', margin.top + 105)
                .attr('font-size', '9pt')
                .text('Seen cards: ' + data[data.length - 1].seen)

            g.append('text')
                .attr('fill', '#000')
                .attr('x', margin.left)
                .attr('y', margin.top + 120)
                .attr('font-size', '9pt')
                .text('Learned cards: ' + data[data.length - 1].learned)
        })
    }
}
