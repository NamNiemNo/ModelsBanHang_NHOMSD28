(function () {
    "use strict";

    const chartStates = new Map();
    const numberFormatter = new Intl.NumberFormat("vi-VN");

    const barColors = [
        ["#4f46e5", "#7c3aed"],
        ["#0891b2", "#06b6d4"],
        ["#059669", "#10b981"],
        ["#64748b", "#94a3b8"],
        ["#64748b", "#94a3b8"],
        ["#64748b", "#94a3b8"],
        ["#64748b", "#94a3b8"]
    ];

    function draw(canvasId, labels, values) {
        const canvas = document.getElementById(canvasId);

        if (!canvas) {
            return;
        }

        destroy(canvasId);

        const safeLabels = Array.isArray(labels)
            ? labels.map(function (label) {
                return label == null
                    ? "Sản phẩm chưa có tên"
                    : String(label);
            })
            : [];

        const safeValues = Array.isArray(values)
            ? values.map(function (value) {
                return Math.max(0, Number(value) || 0);
            })
            : [];

        const itemCount = Math.min(
            safeLabels.length,
            safeValues.length
        );

        const state = {
            canvas: canvas,
            labels: safeLabels.slice(0, itemCount),
            values: safeValues.slice(0, itemCount),
            bars: [],
            hoverIndex: -1,
            frameId: 0,
            width: 0,
            height: 0,
            tooltip: createTooltip(canvas.parentElement)
        };

        state.render = function () {
            renderHorizontalBars(state);
        };

        state.queueRender = function () {
            if (state.frameId) {
                cancelAnimationFrame(state.frameId);
            }

            state.frameId = requestAnimationFrame(function () {
                state.frameId = 0;
                state.render();
            });
        };

        state.onMouseMove = function (event) {
            handlePointerMove(state, event);
        };

        state.onMouseLeave = function () {
            hideTooltip(state);

            if (state.hoverIndex !== -1) {
                state.hoverIndex = -1;
                state.queueRender();
            }
        };

        canvas.addEventListener(
            "mousemove",
            state.onMouseMove
        );

        canvas.addEventListener(
            "mouseleave",
            state.onMouseLeave
        );

        if (typeof ResizeObserver !== "undefined") {
            state.observer =
                new ResizeObserver(state.queueRender);

            state.observer.observe(
                canvas.parentElement || canvas
            );
        } else {
            state.onWindowResize =
                state.queueRender;

            window.addEventListener(
                "resize",
                state.onWindowResize
            );
        }

        chartStates.set(canvasId, state);
        state.render();
    }

    function renderHorizontalBars(state) {
        const canvas = state.canvas;
        const parent = canvas.parentElement;
        const labels = state.labels;
        const values = state.values;

        const parentWidth = parent
            ? parent.clientWidth - 32
            : 760;

        // Trên màn hình nhỏ, biểu đồ sẽ cuộn ngang
        // trong khung thay vì làm vỡ toàn bộ giao diện.
        const cssWidth = Math.max(
            540,
            parentWidth
        );

        const cssHeight = Math.max(
            340,
            labels.length * 54 + 92
        );

        const pixelRatio = Math.min(
            window.devicePixelRatio || 1,
            2
        );

        state.width = cssWidth;
        state.height = cssHeight;

        canvas.width = Math.round(
            cssWidth * pixelRatio
        );

        canvas.height = Math.round(
            cssHeight * pixelRatio
        );

        canvas.style.width = cssWidth + "px";
        canvas.style.height = cssHeight + "px";

        const context = canvas.getContext("2d");

        if (!context) {
            return;
        }

        context.setTransform(
            pixelRatio,
            0,
            0,
            pixelRatio,
            0,
            0
        );

        context.clearRect(
            0,
            0,
            cssWidth,
            cssHeight
        );

        if (
            labels.length === 0 ||
            values.length === 0
        ) {
            state.bars = [];

            drawEmptyState(
                context,
                cssWidth,
                cssHeight
            );

            return;
        }

        const left = Math.min(
            210,
            Math.max(155, cssWidth * 0.29)
        );

        const right = 44;
        const top = 36;
        const bottom = 40;

        const chartWidth = Math.max(
            160,
            cssWidth - left - right
        );

        const chartHeight =
            cssHeight - top - bottom;

        const rowHeight =
            chartHeight / labels.length;

        const barHeight = Math.min(
            28,
            rowHeight * 0.56
        );

        const maxValue = Math.max.apply(
            null,
            values.concat([1])
        );

        const axis = createNiceAxis(
            maxValue,
            5
        );

        drawRows(
            context,
            left,
            top,
            chartWidth,
            chartHeight,
            rowHeight,
            labels.length
        );

        drawGrid(
            context,
            left,
            top,
            chartWidth,
            chartHeight,
            axis.max,
            axis.steps
        );

        state.bars = [];

        labels.forEach(function (label, index) {
            const value = values[index];

            const centerY =
                top +
                rowHeight * index +
                rowHeight / 2;

            const barY =
                centerY - barHeight / 2;

            const barWidth =
                value / axis.max * chartWidth;

            const isHovered =
                state.hoverIndex === index;

            const colors =
                barColors[index] ||
                barColors[barColors.length - 1];

            state.bars.push({
                x: left,
                y: centerY - rowHeight / 2,
                width: chartWidth,
                height: rowHeight,
                label: label,
                value: value
            });

            drawProductLabel(
                context,
                label,
                index,
                left,
                centerY,
                isHovered
            );

            context.save();

            if (isHovered) {
                context.shadowColor =
                    hexToRgba(colors[0], 0.25);

                context.shadowBlur = 14;
                context.shadowOffsetY = 4;
            }

            context.fillStyle = isHovered
                ? "#e8eaff"
                : "#eef1f7";

            drawRoundedRectangle(
                context,
                left,
                barY,
                chartWidth,
                barHeight,
                barHeight / 2
            );

            context.fill();

            if (barWidth > 0) {
                const gradient =
                    context.createLinearGradient(
                        left,
                        0,
                        left + chartWidth,
                        0
                    );

                gradient.addColorStop(
                    0,
                    colors[0]
                );

                gradient.addColorStop(
                    1,
                    colors[1]
                );

                context.fillStyle = gradient;

                drawRoundedRectangle(
                    context,
                    left,
                    barY,
                    Math.max(
                        Math.min(
                            barHeight,
                            chartWidth
                        ),
                        barWidth
                    ),
                    barHeight,
                    barHeight / 2
                );

                context.fill();
            }

            context.restore();

            drawValue(
                context,
                value,
                left,
                centerY,
                barWidth,
                chartWidth,
                colors[0]
            );
        });

        canvas.style.cursor =
            state.hoverIndex >= 0
                ? "pointer"
                : "default";
    }

    function createNiceAxis(
        maxValue,
        preferredSteps
    ) {
        const paddedMax = Math.max(
            1,
            maxValue * 1.1
        );

        const rawStep =
            paddedMax / preferredSteps;

        const magnitude = Math.pow(
            10,
            Math.floor(Math.log10(rawStep))
        );

        const normalized =
            rawStep / magnitude;

        let niceNormalized;

        if (normalized <= 1) {
            niceNormalized = 1;
        } else if (normalized <= 2) {
            niceNormalized = 2;
        } else if (normalized <= 5) {
            niceNormalized = 5;
        } else {
            niceNormalized = 10;
        }

        const step = Math.max(
            1,
            niceNormalized * magnitude
        );

        const axisMax = Math.max(
            step,
            Math.ceil(paddedMax / step) * step
        );

        return {
            max: axisMax,
            steps: Math.max(
                1,
                Math.round(axisMax / step)
            )
        };
    }

    function drawRows(
        context,
        left,
        top,
        width,
        height,
        rowHeight,
        count
    ) {
        for (
            let index = 0;
            index < count;
            index += 1
        ) {
            if (index % 2 !== 0) {
                continue;
            }

            context.fillStyle =
                "rgba(248, 250, 252, 0.82)";

            drawRoundedRectangle(
                context,
                left - 8,
                top + rowHeight * index + 3,
                width + 16,
                Math.max(
                    0,
                    rowHeight - 6
                ),
                9
            );

            context.fill();
        }
    }

    function drawGrid(
        context,
        left,
        top,
        width,
        height,
        maxValue,
        steps
    ) {
        context.save();

        context.lineWidth = 1;
        context.textAlign = "center";
        context.textBaseline = "top";
        context.font =
            "600 10px Inter, system-ui, sans-serif";

        for (
            let index = 0;
            index <= steps;
            index += 1
        ) {
            const ratio = index / steps;
            const x = left + width * ratio;

            context.beginPath();

            context.setLineDash(
                index === 0
                    ? []
                    : [4, 5]
            );

            context.moveTo(x, top);

            context.lineTo(
                x,
                top + height
            );

            context.strokeStyle =
                index === 0
                    ? "#cbd5e1"
                    : "#e7eaf0";

            context.stroke();

            context.fillStyle = "#94a3b8";

            context.fillText(
                numberFormatter.format(
                    Math.round(
                        maxValue * ratio
                    )
                ),
                x,
                top + height + 12
            );
        }

        context.restore();
    }

    function drawProductLabel(
        context,
        label,
        index,
        left,
        centerY,
        isHovered
    ) {
        const badgeX = 18;
        const badgeSize = 24;

        const badgeColors = [
            "#4f46e5",
            "#0891b2",
            "#059669"
        ];

        context.save();

        context.fillStyle =
            badgeColors[index] ||
            "#e2e8f0";

        drawRoundedRectangle(
            context,
            badgeX,
            centerY - badgeSize / 2,
            badgeSize,
            badgeSize,
            8
        );

        context.fill();

        context.textAlign = "center";
        context.textBaseline = "middle";

        context.fillStyle =
            index < 3
                ? "#ffffff"
                : "#64748b";

        context.font =
            "800 10px Inter, system-ui, sans-serif";

        context.fillText(
            String(index + 1),
            badgeX + badgeSize / 2,
            centerY + 0.5
        );

        context.textAlign = "right";

        context.fillStyle = isHovered
            ? "#312e81"
            : "#334155";

        context.font = isHovered
            ? "750 11px Inter, system-ui, sans-serif"
            : "650 11px Inter, system-ui, sans-serif";

        context.fillText(
            shortenLabel(
                label,
                left >= 190 ? 24 : 18
            ),
            left - 12,
            centerY
        );

        context.restore();
    }

    function drawValue(
        context,
        value,
        left,
        centerY,
        barWidth,
        chartWidth,
        color
    ) {
        const text =
            numberFormatter.format(value) +
            " SP";

        context.save();

        context.textBaseline = "middle";
        context.font =
            "800 10px Inter, system-ui, sans-serif";

        const textWidth =
            context.measureText(text).width;

        const canPlaceInside =
            barWidth >= textWidth + 28;

        if (canPlaceInside) {
            context.textAlign = "right";
            context.fillStyle = "#ffffff";

            context.fillText(
                text,
                left + barWidth - 12,
                centerY + 0.5
            );
        } else {
            context.textAlign = "left";
            context.fillStyle = color;

            context.fillText(
                text,
                Math.min(
                    left + barWidth + 9,
                    left +
                    chartWidth -
                    textWidth
                ),
                centerY + 0.5
            );
        }

        context.restore();
    }

    function drawEmptyState(
        context,
        width,
        height
    ) {
        const centerX = width / 2;
        const centerY = height / 2 - 8;

        context.save();

        context.fillStyle = "#eef2ff";
        context.beginPath();

        context.arc(
            centerX,
            centerY - 18,
            30,
            0,
            Math.PI * 2
        );

        context.fill();

        context.strokeStyle = "#818cf8";
        context.lineWidth = 4;
        context.lineCap = "round";
        context.beginPath();

        context.moveTo(
            centerX - 13,
            centerY - 8
        );

        context.lineTo(
            centerX - 13,
            centerY - 22
        );

        context.moveTo(
            centerX,
            centerY - 8
        );

        context.lineTo(
            centerX,
            centerY - 30
        );

        context.moveTo(
            centerX + 13,
            centerY - 8
        );

        context.lineTo(
            centerX + 13,
            centerY - 38
        );

        context.stroke();

        context.textAlign = "center";
        context.textBaseline = "middle";
        context.fillStyle = "#334155";

        context.font =
            "750 13px Inter, system-ui, sans-serif";

        context.fillText(
            "Chưa có dữ liệu bán hàng",
            centerX,
            centerY + 34
        );

        context.fillStyle = "#94a3b8";

        context.font =
            "500 11px Inter, system-ui, sans-serif";

        context.fillText(
            "Hãy chọn một khoảng thời gian khác",
            centerX,
            centerY + 54
        );

        context.restore();
    }

    function handlePointerMove(
        state,
        event
    ) {
        const rectangle =
            state.canvas.getBoundingClientRect();

        const x =
            event.clientX - rectangle.left;

        const y =
            event.clientY - rectangle.top;

        let nextIndex = -1;

        for (
            let index = 0;
            index < state.bars.length;
            index += 1
        ) {
            const bar = state.bars[index];

            if (
                x >= bar.x &&
                x <= bar.x + bar.width &&
                y >= bar.y &&
                y <= bar.y + bar.height
            ) {
                nextIndex = index;
                break;
            }
        }

        if (
            nextIndex !== state.hoverIndex
        ) {
            state.hoverIndex = nextIndex;
            state.queueRender();
        }

        if (nextIndex >= 0) {
            showTooltip(
                state,
                nextIndex,
                event.clientX,
                event.clientY
            );
        } else {
            hideTooltip(state);
        }
    }

    function createTooltip(parent) {
        if (!parent) {
            return null;
        }

        const tooltip =
            document.createElement("div");

        tooltip.className =
            "tk-chart-tooltip";

        tooltip.setAttribute(
            "role",
            "tooltip"
        );

        parent.appendChild(tooltip);

        return tooltip;
    }

    function showTooltip(
        state,
        index,
        clientX,
        clientY
    ) {
        const tooltip = state.tooltip;

        if (!tooltip) {
            return;
        }

        tooltip.replaceChildren();

        const title =
            document.createElement("strong");

        title.textContent =
            state.labels[index];

        const value =
            document.createElement("span");

        value.textContent =
            numberFormatter.format(
                state.values[index]
            ) +
            " sản phẩm đã bán";

        tooltip.append(title, value);
        tooltip.classList.add("show");

        const parent =
            tooltip.parentElement;

        const parentRect =
            parent.getBoundingClientRect();

        const tooltipRect =
            tooltip.getBoundingClientRect();

        const scrollLeft =
            parent.scrollLeft || 0;

        const scrollTop =
            parent.scrollTop || 0;

        const visibleRight =
            scrollLeft +
            parent.clientWidth -
            8;

        const visibleBottom =
            scrollTop +
            parent.clientHeight -
            8;

        let left =
            clientX -
            parentRect.left +
            scrollLeft +
            14;

        let top =
            clientY -
            parentRect.top +
            scrollTop +
            14;

        if (
            left + tooltipRect.width >
            visibleRight
        ) {
            left =
                clientX -
                parentRect.left +
                scrollLeft -
                tooltipRect.width -
                14;
        }

        if (
            top + tooltipRect.height >
            visibleBottom
        ) {
            top =
                clientY -
                parentRect.top +
                scrollTop -
                tooltipRect.height -
                14;
        }

        tooltip.style.left =
            Math.max(
                scrollLeft + 8,
                left
            ) + "px";

        tooltip.style.top =
            Math.max(
                scrollTop + 8,
                top
            ) + "px";
    }

    function hideTooltip(state) {
        if (state.tooltip) {
            state.tooltip.classList.remove(
                "show"
            );
        }
    }

    function drawRoundedRectangle(
        context,
        x,
        y,
        width,
        height,
        radius
    ) {
        const safeWidth =
            Math.max(0, width);

        const safeHeight =
            Math.max(0, height);

        const safeRadius = Math.min(
            radius,
            safeWidth / 2,
            safeHeight / 2
        );

        context.beginPath();

        context.moveTo(
            x + safeRadius,
            y
        );

        context.lineTo(
            x + safeWidth - safeRadius,
            y
        );

        context.quadraticCurveTo(
            x + safeWidth,
            y,
            x + safeWidth,
            y + safeRadius
        );

        context.lineTo(
            x + safeWidth,
            y + safeHeight - safeRadius
        );

        context.quadraticCurveTo(
            x + safeWidth,
            y + safeHeight,
            x + safeWidth - safeRadius,
            y + safeHeight
        );

        context.lineTo(
            x + safeRadius,
            y + safeHeight
        );

        context.quadraticCurveTo(
            x,
            y + safeHeight,
            x,
            y + safeHeight - safeRadius
        );

        context.lineTo(
            x,
            y + safeRadius
        );

        context.quadraticCurveTo(
            x,
            y,
            x + safeRadius,
            y
        );

        context.closePath();
    }

    function shortenLabel(
        value,
        maxLength
    ) {
        const text =
            value == null
                ? ""
                : String(value);

        if (text.length <= maxLength) {
            return text;
        }

        return text.substring(
            0,
            maxLength - 1
        ) + "…";
    }

    function hexToRgba(
        hex,
        alpha
    ) {
        const value =
            hex.replace("#", "");

        const red = parseInt(
            value.substring(0, 2),
            16
        );

        const green = parseInt(
            value.substring(2, 4),
            16
        );

        const blue = parseInt(
            value.substring(4, 6),
            16
        );

        return (
            "rgba(" +
            red +
            ", " +
            green +
            ", " +
            blue +
            ", " +
            alpha +
            ")"
        );
    }

    function destroy(canvasId) {
        const state =
            chartStates.get(canvasId);

        if (!state) {
            return;
        }

        if (state.observer) {
            state.observer.disconnect();
        }

        if (state.onWindowResize) {
            window.removeEventListener(
                "resize",
                state.onWindowResize
            );
        }

        if (state.frameId) {
            cancelAnimationFrame(
                state.frameId
            );
        }

        state.canvas.removeEventListener(
            "mousemove",
            state.onMouseMove
        );

        state.canvas.removeEventListener(
            "mouseleave",
            state.onMouseLeave
        );

        if (state.tooltip) {
            state.tooltip.remove();
        }

        chartStates.delete(canvasId);
    }

    window.thongKeChart = {
        draw: draw,
        destroy: destroy
    };
})();