const forecast = {
    linearRegression: (y, x) => {
        const lr = {}
        const n = y.length
        const sum_x = 0
        const sum_y = 0
        const sum_xy = 0
        const sum_xx = 0
        const sum_yy = 0

        for (let i = 0; i < y.length; i++) {
            sum_x += x[i]
            sum_y += y[i]
            sum_xy += (x[i] * y[i])
            sum_xx += (x[i] * x[i])
            sum_yy += (y[i] * y[i])
        }

        lr['slope'] = (n * sum_xy - sum_x * sum_y) / (n * sum_xx - sum_x * sum_x)
        lr['intercept'] = (sum_y - lr.slope * sum_x) / n
        lr['r2'] = Math.pow((n * sum_xy - sum_x * sum_y) / Math.sqrt((n * sum_xx - sum_x * sum_x) * (n * sum_yy - sum_y * sum_y)), 2)

        return lr
    },
    render: (url, selector) => {
        const margin = {
            top: 20,
            right: 20,
            bottom: 80,
            left: 50
        }
        const width = 1200 - margin.left - margin.right
        const height = 600 - margin.top - margin.bottom

        const svg = d3.select('#chart-forecast')
            .attr('width', width + margin.left + margin.right)
            .attr('height', height + margin.top + margin.bottom)
            .append('g')
            .attr('transform', `translate(${margin.left},${margin.top})`)

        d3.json(url, (error, data) => {
            data.forEach((d) => {
                const date = new Date(d.date)
                date.setHours(0, 0, 0, 0)
                d.date = date
            })

            const today = new Date().setHours(0, 0, 0, 0)

            if (d3.min(data, (d) => d.date) > today) {
                data = [{ date: today, count: 0, average: 0 }].concat(data)
            }

            const extent = d3.extent(data.map(d => d.date))

            const range = d3.timeDays(extent[0], new Date(extent[1])
                .setDate(extent[1].getDate() + 1))

            data = range.map(d => {
                for (let index = 0; index < data.length; index++) {
                    const element = data[index]
                    if (element.date.toString() === d.toString()) {
                        return element
                    }
                }
                return {
                    date: d,
                    count: 0,
                    average: 0
                };
            })

            const x = d3.scaleBand()
                .rangeRound([0, width], .05)
                .padding(.1)
                .domain(data.map(d => d.date))

            const y = d3.scaleLinear()
                .range([height, 0])
                .domain([0, d3.max(data, d => Math.max(d.count, 100))])

            const xAxis = d3.axisBottom()
                .scale(x)
                .tickFormat(d3.timeFormat('%Y-%m-%d %a'))

            const yAxis = d3.axisLeft()
                .scale(y)
                .ticks(10)

            const colors = [
                '#00C852',
                '#00C922',
                '#0DCA00',
                '#3FCB00',
                '#71CD00',
                '#A3CE00',
                '#CFC800',
                '#D19700',
                '#D26500',
                '#D33300',
                '#D50000'
            ]

            svg.append('g')
                .attr('transform', `translate(0,${height})`)
                .call(xAxis)
                .selectAll('text')
                .style('text-anchor', 'end')
                .attr('font-size', '8')
                .attr('dx', '-.8em')
                .attr('dy', '-.55em')
                .attr('transform', 'rotate(-90)')

            svg.append('g')
                .call(yAxis)
                .append('text')

            svg.selectAll('bar')
                .data(data)
                .enter()
                .append('rect')
                .style('fill', d => colors[Math.round(d.average)])
                .attr('x', d => x(d.date))
                .attr('width', d => x.bandwidth())
                .attr('y', d => y(d.count))
                .attr('height', d => height - y(d.count))
                .append('title')
                .text(d => Math.round(d.average * 10) / 10)

            svg.selectAll('bar')
                .data(data)
                .enter()
                .append('text')
                .attr('x', d => x(d.date) + x.bandwidth() / 2)
                .attr('y', d => y(d.count) - 3)
                .attr('text-anchor', 'middle')
                .attr('width', x.bandwidth())
                .attr('font-size', '8')
                .text(d => d.count)
        })
    }
}
