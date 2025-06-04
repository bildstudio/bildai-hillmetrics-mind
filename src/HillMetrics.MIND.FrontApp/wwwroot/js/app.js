// Workflow animations
window.initWorkflowAnimations = function () {
    console.log("Initializing workflow animations");
    updateWorkflowAnimations();
};

window.updateWorkflowAnimations = function () {
    // Add animate class to in-progress paths
    const inProgressPaths = document.querySelectorAll('.path-inprogress');
    inProgressPaths.forEach(path => {
        path.classList.add('animate');
    });

    // Add transitions for hovering over workflow steps
    const workflowSteps = document.querySelectorAll('.workflow-step');
    workflowSteps.forEach(step => {
        step.addEventListener('mouseenter', () => {
            step.style.zIndex = '10';
        });
        step.addEventListener('mouseleave', () => {
            step.style.zIndex = '1';
        });
    });

    // Add tooltips to metric badges
    const badges = document.querySelectorAll('.badge');
    badges.forEach(badge => {
        // Get the badge text content and class
        const value = badge.textContent;
        const className = Array.from(badge.classList)
            .find(cls => ['success', 'info', 'warning', 'error'].includes(cls));

        // Set title based on badge type
        if (className === 'success') {
            badge.title = `${value.replace('+', '')} records added`;
        } else if (className === 'info') {
            badge.title = `${value.replace('~', '')} records updated`;
        } else if (className === 'warning') {
            badge.title = `${value} records ignored (already exists)`;
        } else if (className === 'error') {
            badge.title = `${value.replace('!', '')} records with errors`;
        }
    });
};

// File download functionality
window.downloadFile = function (filename, content, contentType) {
    // Create a blob with the content
    const blob = new Blob([content], { type: contentType || 'text/plain' });

    // Create a temporary URL for the blob
    const url = window.URL.createObjectURL(blob);

    // Create a temporary anchor element for download
    const anchor = document.createElement('a');
    anchor.href = url;
    anchor.download = filename;
    anchor.style.display = 'none';

    // Add to DOM, click, and remove
    document.body.appendChild(anchor);
    anchor.click();
    document.body.removeChild(anchor);

    // Clean up the URL
    window.URL.revokeObjectURL(url);
};

// MathJax functions required by MudMarkdown
window.refreshMathJaxScript = function() {
    if (window.MathJax && window.MathJax.typesetPromise) {
        return window.MathJax.typesetPromise();
    }
    return Promise.resolve();
};

window.appendMathJaxScript = function() {
    if (!document.getElementById('mathjax-script')) {
        const script = document.createElement('script');
        script.id = 'mathjax-script';
        script.src = 'https://polyfill.io/v3/polyfill.min.js?features=es6';
        script.onload = function() {
            const mathJaxScript = document.createElement('script');
            mathJaxScript.src = 'https://cdn.jsdelivr.net/npm/mathjax@3/es5/tex-mml-chtml.js';
            mathJaxScript.onload = function() {
                // Ensure MathJax is configured after loading
                if (window.MathJax) {
                    window.MathJax.startup.defaultReady();
                }
            };
            document.head.appendChild(mathJaxScript);
        };
        document.head.appendChild(script);
    }
};