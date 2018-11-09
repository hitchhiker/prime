import { OhlcRecord } from "./ohlc/ohlc-record";
import * as d3 from "d3";

class OhlcItem {
    ohlc: OhlcRecord;
    posX: number;
}

export class ChartCore {
    private _chartData: OhlcItem[];
    private _chartDataInView: OhlcItem[];

    private svg;
    private g;

    private _chartOffsetX: number = -600;

    constructor(selector: string) {
        this.initialize(selector);
    }

    private _sizing = {
        margin: {
            top: 10, right: 10, bottom: 10, left: 10
        },
        bars: {
            width: 10,
            gap: 3
        },
        height: 450,
        width: 600
    };

    public set chartOffsetX(v: number) {
        this._chartOffsetX = v;

        this.render();
    }
    public get chartOffsetX() : number {
        return this._chartOffsetX;
    }
    
    public set barWidth(v: number) {
        let displacement = this.calcZoomOffsetDisplacement(v);
        console.log([displacement, this.chartOffsetX]);
        
        this._sizing.bars.width = v;

        this.chartOffsetX -= displacement;
        this.render();
    }
    public get barWidth(): number {
        return this._sizing.bars.width;
    }

    public initialize(selector: string) {
        this.svg = d3.select(selector)
            .append("svg").attr("width", this._sizing.width).attr("height", this._sizing.height);

        this.svg.append("rect").attr("width", "100%").attr("height", "100%").attr("fill", "rgb(30, 30, 30)");
        this.g = this.svg.append("g");
    }

    public setData(data: OhlcRecord[]) {
        this._chartData = data.map((x, i) => {
            let r: OhlcItem = { ohlc: x, posX: this.getBarPosX(i) };
            return r;
        });

        //this.viewPort.x1 = this.getXbyIndex(data.length - 1);
    }

    private recalcItemsPosX(data: OhlcItem[]): OhlcItem[] {
        data.forEach((v, i) => {
            v.posX = this.getBarPosX(i);
        });

        return data;
    }

    public getBarPosX(i: number): number {
        return i * (this._sizing.bars.width + this._sizing.bars.gap)
    }

    public getInView(data: OhlcItem[]): OhlcItem[] {
        let inView = data.filter((record, i) => {
            let startX = -this._chartOffsetX;
            let endX = startX + this._sizing.width;
            let currX = record.posX;
            
            let r = currX >= startX && currX <= endX;

            return r;
        });

        return inView;
    }

    // Calculates center of chart view (SVG view) in chart data coordinates (taking into account chart offset X).
    private calcZoomOffsetDisplacement(newWidth: number): number {
        let offsetAndWidth = -this._chartOffsetX + this._sizing.width;
        let itemsInViewOld = offsetAndWidth / (this._sizing.bars.width + this._sizing.bars.gap);
        let itemsInViewNew = offsetAndWidth / (newWidth + this._sizing.bars.gap);
        let rel = itemsInViewOld / itemsInViewNew ;
        let diff = itemsInViewNew - itemsInViewOld;
        let partDiff = (rel * offsetAndWidth - offsetAndWidth) * ((offsetAndWidth - (this._sizing.width / 2)) / offsetAndWidth);

        return partDiff;
    }

    public render() {
        let self = this;
        let svg = this.svg;
        let allData = this.recalcItemsPosX(this._chartData);
        let sizing = this._sizing;
        let g = this.g;

        this._chartDataInView = this.getInView(allData);
        let data = this._chartDataInView;

        let yScaleMin: number = d3.min(data, (d: OhlcItem) => { return d.ohlc.low; });
        let yScaleMax: number = d3.max(data, (d: OhlcItem) => { return d.ohlc.high; });

        // Scales.
        let yScaleRaw = d3.scaleLinear()
            .domain([yScaleMin, yScaleMax]) // // d3.extent(data, (x: OhlcRecord) => { return x.open; })
            .range([sizing.height, 0]);
        let yScale = function (v) {
            return yScaleRaw(v).toFixed(4);
        };

        // Clear everything.
        g.selectAll("*").remove();

        // Populate data.
        let svgData = g
            .selectAll("g")
            .data(data);

        // Add groups.
        let groups = svgData.enter().append("g")
            .attr("transform", (item: OhlcItem, i) => {
                return `translate(${(item.posX + self._chartOffsetX)}, ${yScale(item.ohlc.low)})`;
            });

        groups.append("line");
        groups.append("rect");

        // Axis.
        // let xAxis = d3.axisBottom(yScaleRaw);
        // svg.append("g").call(xAxis);

        //let gW = g.node().getBBox().width;

        // Zoom.
        // svg
        //   .call(d3.zoom()
        //     .scaleExtent([1 / 2, 4])
        //     .translateExtent([[-100, svgH / 2], [gW + 100, svgH / 2]])
        //     .on("zoom", zoomed));

        // function zoomed() {
        //   //console.log(d3.event.transform);
        // }

        // Line test.
        // function lineTest() {
        //     let line = d3.line()
        //         .x((d: OhlcRecord, i) => {
        //             return i * 10;
        //         })
        //         .y((da: OhlcRecord, i) => {
        //             return yScaleRaw(da.open);
        //         });
        //     svg.append("path")
        //         .attr("stroke-width", 1)
        //         .attr("stroke", "white")
        //         .attr("fill", "transparent")
        //         .attr("d", line(data));
        // }

        // Process lines.
        groups.selectAll("line")
            .attr("transform", (x: OhlcItem, i) => {
                return `translate(${(sizing.bars.width / 2)}, ${0})`;
            })
            .attr("x1", 0).attr("x2", 0)
            .attr("y1", 0)
            .attr("y2", (x: OhlcItem, i) => {
                return (yScale(x.ohlc.high) - yScale(x.ohlc.low));
            })
            .attr("stroke-width", 1)
            .attr("stroke", (o: OhlcItem) => {
                return getColor(o.ohlc);
            });

        // Process rects.
        groups.selectAll("rect")
            .attr("width", sizing.bars.width)
            .attr("height", (o: OhlcItem) => {
                return Math.abs(yScale(o.ohlc.open) - yScale(o.ohlc.close));
            })
            .attr("fill", (o: OhlcItem) => {
                return getColor(o.ohlc);
            })
            .attr("x", 0)
            .attr("y", (x: OhlcItem, i) => {
                let openLow = yScale(x.ohlc.open) - yScale(x.ohlc.low);
                let closeLow = yScale(x.ohlc.close) - yScale(x.ohlc.low)
                return openLow < closeLow ? openLow : closeLow;
            });

        svgData.exit().remove();

        function getColor(o: OhlcRecord): string {
            return o.open - o.close >= 0 ? "red" : "green";
        }
    }
}
