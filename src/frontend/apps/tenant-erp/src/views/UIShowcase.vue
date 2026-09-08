<template>
  <div class="ui-showcase">
    <h1>Khet-360 UI Components Showcase</h1>
    <p>Demonstration of the modern, accessible UI components with smooth animations and transitions.</p>
    
    <div class="section">
      <h2>Buttons</h2>
      <div class="button-group">
        <KButton variant="primary" @click="alert('Primary button clicked')">Primary</KButton>
        <KButton variant="secondary" @click="alert('Secondary button clicked')">Secondary</KButton>
        <KButton variant="success" @click="alert('Success button clicked')">Success</KButton>
        <KButton variant="warning" @click="alert('Warning button clicked')">Warning</KButton>
        <KButton variant="danger" @click="alert('Danger button clicked')">Danger</KButton>
        <KButton variant="outline" @click="alert('Outline button clicked')">Outline</KButton>
        <KButton variant="ghost" @click="alert('Ghost button clicked')">Ghost</KButton>
        <KButton variant="primary" icon="⚡" @click="alert('Icon button clicked')">Icon Button</KButton>
        <KButton variant="primary" :loading="true">Loading...</KButton>
        <KButton variant="primary" disabled>Disabled Button</KButton>
      </div>
      
      <div class="button-group">
        <KButton variant="primary" size="small">Small</KButton>
        <KButton variant="primary" size="medium">Medium</KButton>
        <KButton variant="primary" size="large">Large</KButton>
      </div>
    </div>
    
    <div class="section">
      <h2>Inputs</h2>
      <div class="input-group">
        <KInput label="Text Input" placeholder="Enter text" />
        <KInput label="Email Input" type="email" placeholder="Enter email" />
        <KInput label="Password Input" type="password" placeholder="Enter password" />
        <KInput label="Number Input" type="number" placeholder="Enter number" />
        <KInput label="Disabled Input" placeholder="This is disabled" disabled />
        <KInput label="With Clear" placeholder="Type to see clear button" showClear />
        <KInput label="With Icon" placeholder="Search..." icon="🔍" />
      </div>
    </div>
    
    <div class="section">
      <h2>Selects</h2>
      <div class="select-group">
        <KInput label="Standard Select" placeholder="Select an option">
          <template #slot>
            <KSelect placeholder="Choose an option">
              <template #slot>
                <option value="option1">Option 1</option>
                <option value="option2">Option 2</option>
                <option value="option3">Option 3</option>
              </template>
            </KSelect>
          </template>
        </KInput>
      </div>
    </div>
    
    <div class="section">
      <h2>Textarea</h2>
      <div class="textarea-group">
        <KTextarea label="Bio" placeholder="Tell us about yourself..." rows="4" />
        <KTextarea label="With Clear" placeholder="Type to see clear button" showClear rows="3" />
        <KTextarea label="Disabled" placeholder="This is disabled" disabled rows="3" />
      </div>
    </div>
    
    <div class="section">
      <h2>Badges</h2>
      <div class="badge-group">
        <KBadge variant="primary">Primary</KBadge>
        <KBadge variant="secondary">Secondary</KBadge>
        <KBadge variant="success">Success</KBadge>
        <KBadge variant="warning">Warning</KBadge>
        <KBadge variant="error">Error</KBadge>
        <KBadge variant="info">Info</KBadge>
        
        <KBadge variant="primary" size="small">Small</KBadge>
        <KBadge variant="primary" size="medium">Medium</KBadge>
        <KBadge variant="primary" size="large">Large</KBadge>
        
        <KBadge variant="primary" dot />
        <KBadge variant="success" dot />
        <KBadge variant="error" dot />
      </div>
    </div>
    
    <div class="section">
      <h2>Cards</h2>
      <div class="card-group">
        <KCard variant="default" class="card">
          <template #header>
            <h3 class="card-title">Default Card</h3>
          </template>
          This is a default card with subtle elevation.
          <template #footer>
            <KButton variant="outline" size="small">Action</KButton>
          </template>
        </KCard>
        
        <KCard variant="elevated" class="card">
          <template #header>
            <h3 class="card-title">Elevated Card</h3>
          </template>
          This card has more pronounced shadow for emphasis.
          <template #footer>
            <KButton variant="primary" size="small">Action</KButton>
          </template>
        </KCard>
        
        <KCard variant="outlined" class="card">
          <template #header>
            <h3 class="card-title">Outlined Card</h3>
          </template>
          This card has a border but no background fill.
          <template #footer>
            <KButton variant="outline" size="small">Action</KButton>
          </template>
        </KCard>
        
        <KCard variant="filled" class="card" class="card">
          <template #header>
            <h3 class="card-title">Filled Card</h3>
          </template>
          This card has a background fill for emphasis.
          <template #footer>
            <KButton variant="primary" size="small">Action</KButton>
          </template>
        </KCard>
        
        <KCard variant="default" hoverable class="card">
          <template #header>
            <h3 class="card-title">Hoverable Card</h3>
          </template>
          This card lifts up when hovered.
          <template #footer>
            <KButton variant="outline" size="small">Action</KButton>
          </template>
        </KCard>
      </div>
    </div>
    
    <div class="section">
      <h2>Alerts</h2>
      <div class="alert-group">
        <KAlert variant="info">
          <strong>Heads up!</strong> This alert needs your attention, but it's not super important.
        </KAlert>
        <KAlert variant="success">
          <strong>Well done!</strong> You successfully read this important alert message.
        </KAlert>
        <KAlert variant="warning">
          <strong>Warning!</strong> Better check yourself, you're not looking too good.
        </KAlert>
        <KAlert variant="error">
          <strong>Oh snap!</strong> Change a few things up and try submitting again.
        </KAlert>
        
        <KAlert variant="success" :closable="true">
          <strong>Well done!</strong> You successfully read this important alert message.
          <template #footer>
            <KButton variant="outline" size="small" @click="alertClosed = true">Got it</KButton>
          </template>
        </KAlert>
      </div>
    </div>
    
    <div class="section">
      <h2>Dialogs</h2>
      <div class="dialog-group">
        <KButton variant="primary" @click="showDialog = true">Open Dialog</KButton>
        
        <KDialog v-model:show="showDialog" title="Confirmation" @close="showDialog = false">
          <p>Are you sure you want to perform this action?</p>
          <p>This action cannot be undone.</p>
          
          <template #footer>
            <KButton variant="secondary" @click="showDialog = false">Cancel</KButton>
            <KButton variant="danger" @click="showDialog = false; alert('Action confirmed!')">Confirm</KButton>
          </template>
        </KDialog>
      </div>
    </div>
    
    <div class="section">
      <h2>Feedback Components</h2>
      <div class="feedback-group">
        <KSpinner size="small" />
        <KSpinner size="medium" />
        <KSpinner size="large" />
        
        <KProgressBar :percent="25" label="Quarter Complete" />
        <KProgressBar :percent="50" label="Halfway There" />
        <KProgressBar :percent="75" label="Almost Done" variant="success" />
        <KProgressBar :percent="100" label="Completed!" variant="success" :showLabel="false" />
        
        <KProgressBar :percent="60" label="Processing" variant="warning" />
        <KProgressBar :percent="80" label="Almost Done" variant="error" />
      </div>
    </div>
    
    <div class="section">
      <h2>Tooltips</h2>
      <div class="tooltip-group">
        <KButton variant="outline">
          <KTooltip placement="top" content="This is a tooltip on top">
            Top Tooltip
          </KTooltip>
        </KButton>
        
        <KButton variant="outline">
          <KTooltip placement="right" content="This is a tooltip on the right">
            Right Tooltip
          </KTooltip>
        </KButton>
        
        <KButton variant="outline">
          <KTooltip placement="bottom" content="This is a tooltip on the bottom">
            Bottom Tooltip
          </KTooltip>
        </KButton>
        
        <KButton variant="outline">
          <KTooltip placement="left" content="This is a tooltip on the left">
            Left Tooltip
          </KTooltip>
        </KButton>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { KButton, KInput, KSelect, KTextarea, KBadge, KCard, KAlert, KDialog, KSpinner, KProgressBar, KTooltip } from '@khet360/ui-shared';

const showDialog = ref(false);
const alertClosed = ref(false);
</script>

<script setup lang="ts">
</script>

<style scoped>
.ui-showcase {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
}

.ui-showcase h1 {
  text-align: center;
  color: #1f2937;
  margin-bottom: 1.5rem;
  font-weight: 700;
}

.ui-showcase p {
  text-align: center;
  color: #6b7280;
  margin-bottom: 2rem;
  font-size: 1.125rem;
}

.section {
  margin-bottom: 3rem;
}

.section h2 {
  color: #374151;
  margin-bottom: 1.5rem;
  font-size: 1.5rem;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.section h2::before {
  content: '';
  width: 4px;
  height: 24px;
  background-color: #3b82f6;
  border-radius: 2px;
}

.button-group,
.input-group,
.select-group,
.textarea-group,
.badge-group,
.card-group,
.alert-group,
.dialog-group,
.feedback-group,
.tooltip-group {
  display: flex;
  flex-wrap: wrap;
  gap: 1rem;
  justify-content: center;
}

.card {
  width: 280px;
}

.alert-group {
  flex-direction: column;
  align-items: stretch;
}

.alert-group .KAlert {
  width: 100%;
  margin-bottom: 0.75rem;
}

.feedback-group {
  flex-wrap: wrap;
  justify-content: center;
  gap: 1.5rem;
}

.tooltip-group {
  display: flex;
  flex-wrap: wrap;
  gap: 2rem;
  justify-content: center;
}

/* Responsive design */
@media (max-width: 768px) {
  .ui-showcase {
    padding: 1rem;
  }
  
  .button-group,
  .input-group,
  .select-group,
  .textarea-group,
  .badge-group,
  .card-group,
  .alert-group,
  .dialog-group,
  .feedback-group,
  .tooltip-group {
    flex-direction: column;
    align-items: stretch;
  }
  
  .card {
    width: 100%;
    max-width: 400px;
  }
  
  .tooltip-group {
    gap: 1rem;
  }
}
</style>