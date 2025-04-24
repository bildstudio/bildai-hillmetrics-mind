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