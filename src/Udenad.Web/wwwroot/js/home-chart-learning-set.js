const learningSet = {
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
            .y0(d => y(d.seen - d.learned))
            .y1(d => y(0))

        d3.json(url, (error, data) => {
            if (error) throw error

            let diffDays;

            data.forEach(value => {
                value.date = new Date(value.date)

                const timeDiff = Math.abs(data[0].date.getTime() - value.date.getTime())
                diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24))

                value.shift = diffDays
            })

            x.domain([data[0].date, data[data.length - 1].date])
            y.domain([0, d3.max(data, d => d.seen)])

            // area path
            g.append('path')
                .datum(data)
                .attr('fill', '#D50000')
                .attr('d', area)

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

            const learning_set = linearRegression(
                data.map(d => d.seen - d.learned),
                data.map(d => d.shift))

            g.append('line')
                .attr('stroke', '#000')
                .attr('stroke-dasharray', '5, 5')
                .attr('x1', 0)
                .attr('y1', y(learning_set.intercept))
                .attr('x2', width)
                .attr('y2', y(learning_set.intercept + learning_set.slope * diffDays))

        })
    }
}