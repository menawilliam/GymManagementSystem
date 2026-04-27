document.addEventListener("DOMContentLoaded", function () {
    const tdeeForm = document.getElementById("tdeeForm");
    const resultContainer = document.getElementById("resultContainer");
    const loadingState = document.getElementById("loadingState");
    const calculateBtn = document.getElementById("calculateBtn");

    if (!tdeeForm) return;

    const btnText = calculateBtn ? calculateBtn.querySelector(".btn-text") : null;
    const btnSpinner = calculateBtn ? calculateBtn.querySelector(".btn-spinner") : null;

    function playSuccessSound() {
        try {
            const AudioCtx = window.AudioContext || window.webkitAudioContext;
            if (!AudioCtx) return;

            const audioCtx = new AudioCtx();
            const osc = audioCtx.createOscillator();
            const gain = audioCtx.createGain();

            osc.type = "sine";
            osc.frequency.setValueAtTime(660, audioCtx.currentTime);
            osc.frequency.setValueAtTime(880, audioCtx.currentTime + 0.08);

            gain.gain.setValueAtTime(0.001, audioCtx.currentTime);
            gain.gain.exponentialRampToValueAtTime(0.08, audioCtx.currentTime + 0.02);
            gain.gain.exponentialRampToValueAtTime(0.001, audioCtx.currentTime + 0.18);

            osc.connect(gain);
            gain.connect(audioCtx.destination);

            osc.start();
            osc.stop(audioCtx.currentTime + 0.18);
        } catch (err) {
            console.log("Audio unavailable", err);
        }
    }

    tdeeForm.addEventListener("submit", function (e) {
        e.preventDefault();

        const data = new FormData(tdeeForm);

        if (resultContainer) {
            resultContainer.innerHTML = "";
        }

        if (loadingState) {
            loadingState.classList.remove("d-none");
        }

        if (calculateBtn) {
            calculateBtn.disabled = true;
        }

        if (btnText) {
            btnText.classList.add("d-none");
        }

        if (btnSpinner) {
            btnSpinner.classList.remove("d-none");
        }

        fetch("/CalorieCalculator/Calculate", {
            method: "POST",
            body: data
        })
            .then(res => res.text())
            .then(html => {
                if (html.includes("Exception") || html.includes("System.")) {
                    console.error(html);

                    if (loadingState) {
                        loadingState.classList.add("d-none");
                    }

                    if (calculateBtn) {
                        calculateBtn.disabled = false;
                    }

                    if (btnText) {
                        btnText.classList.remove("d-none");
                    }

                    if (btnSpinner) {
                        btnSpinner.classList.add("d-none");
                    }

                    if (resultContainer) {
                        resultContainer.innerHTML = `
                            <div class="alert alert-danger rounded-4 mt-3">
                                Something went wrong while calculating.
                            </div>
                        `;
                    }
                    return;
                }

                setTimeout(function () {
                    if (loadingState) {
                        loadingState.classList.add("d-none");
                    }

                    if (calculateBtn) {
                        calculateBtn.disabled = false;
                    }

                    if (btnText) {
                        btnText.classList.remove("d-none");
                    }

                    if (btnSpinner) {
                        btnSpinner.classList.add("d-none");
                    }

                    if (resultContainer) {
                        resultContainer.innerHTML = `<div class="result-fade-in">${html}</div>`;
                    }

                    if (typeof initTdeeResult === "function") {
                        initTdeeResult();
                    }

                    playSuccessSound();

                    const resultCard = document.getElementById("resultCard");
                    if (resultCard) {
                        resultCard.scrollIntoView({
                            behavior: "smooth",
                            block: "start"
                        });
                    }
                }, 900);
            })
            .catch(err => {
                console.error(err);

                if (loadingState) {
                    loadingState.classList.add("d-none");
                }

                if (calculateBtn) {
                    calculateBtn.disabled = false;
                }

                if (btnText) {
                    btnText.classList.remove("d-none");
                }

                if (btnSpinner) {
                    btnSpinner.classList.add("d-none");
                }

                if (resultContainer) {
                    resultContainer.innerHTML = `
                        <div class="alert alert-danger rounded-4 mt-3">
                            Something went wrong while calculating. Please try again.
                        </div>
                    `;
                }
            });
    });
});

function initTdeeResult() {
    const chartCanvas = document.getElementById("macroChart");
    if (!chartCanvas || typeof Chart === "undefined") return;

    const maintenanceProtein = parseFloat(chartCanvas.dataset.maintenanceProtein || 0);
    const maintenanceFats = parseFloat(chartCanvas.dataset.maintenanceFats || 0);
    const maintenanceCarbs = parseFloat(chartCanvas.dataset.maintenanceCarbs || 0);

    const cuttingProtein = parseFloat(chartCanvas.dataset.cuttingProtein || 0);
    const cuttingFats = parseFloat(chartCanvas.dataset.cuttingFats || 0);
    const cuttingCarbs = parseFloat(chartCanvas.dataset.cuttingCarbs || 0);

    const bulkingProtein = parseFloat(chartCanvas.dataset.bulkingProtein || 0);
    const bulkingFats = parseFloat(chartCanvas.dataset.bulkingFats || 0);
    const bulkingCarbs = parseFloat(chartCanvas.dataset.bulkingCarbs || 0);

    const chartTitle = document.getElementById("chartPlanTitle");

    const macroPlans = {
        maintenance: {
            protein: maintenanceProtein,
            fats: maintenanceFats,
            carbs: maintenanceCarbs,
            title: "Maintenance macros"
        },
        cutting: {
            protein: cuttingProtein,
            fats: cuttingFats,
            carbs: cuttingCarbs,
            title: "Cutting macros"
        },
        bulking: {
            protein: bulkingProtein,
            fats: bulkingFats,
            carbs: bulkingCarbs,
            title: "Bulking macros"
        }
    };

    if (window.tdeeChartInstance) {
        window.tdeeChartInstance.destroy();
    }

    window.tdeeChartInstance = new Chart(chartCanvas, {
        type: "doughnut",
        data: {
            labels: ["Protein", "Fats", "Carbs"],
            datasets: [{
                data: [
                    macroPlans.maintenance.protein,
                    macroPlans.maintenance.fats,
                    macroPlans.maintenance.carbs
                ],
                backgroundColor: ["#2f5cff", "#6b3bff", "#8ea2ff"],
                borderWidth: 0
            }]
        },
        options: {
            responsive: true,
            cutout: "68%",
            plugins: {
                legend: {
                    position: "bottom",
                    labels: {
                        color: "#cfd3ff"
                    }
                }
            }
        }
    });

    function updateChart(planKey) {
        const plan = macroPlans[planKey];
        window.tdeeChartInstance.data.datasets[0].data = [
            plan.protein,
            plan.fats,
            plan.carbs
        ];
        window.tdeeChartInstance.update();

        if (chartTitle) {
            chartTitle.textContent = plan.title;
        }
    }

    const maintainTab = document.querySelector('[data-bs-target="#maintain"]');
    if (maintainTab) {
        maintainTab.addEventListener("shown.bs.tab", function () {
            updateChart("maintenance");
        });
    }

    const cutTab = document.querySelector('[data-bs-target="#cut"]');
    if (cutTab) {
        cutTab.addEventListener("shown.bs.tab", function () {
            updateChart("cutting");
        });
    }

    const bulkTab = document.querySelector('[data-bs-target="#bulk"]');
    if (bulkTab) {
        bulkTab.addEventListener("shown.bs.tab", function () {
            updateChart("bulking");
        });
    }
}


// Typing Effect
const text = "Your body can stand anything. Train your mind.";
let i = 0;

function typeEffect() {
    const el = document.getElementById("typingText");
    if (!el) return;

    if (i < text.length) {
        el.innerHTML += text.charAt(i);
        i++;
        setTimeout(typeEffect, 40);
    }
}

document.addEventListener("DOMContentLoaded", typeEffect);


// Counter animation
document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".counter").forEach(el => {
        let target = parseInt(el.innerText);
        let count = 0;

        let interval = setInterval(() => {
            count += Math.ceil(target / 30);

            if (count >= target) {
                el.innerText = target;
                clearInterval(interval);
            } else {
                el.innerText = count;
            }
        }, 30);
    });
});