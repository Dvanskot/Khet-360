<template>
  <div class="home-page">
    <section class="hero">
      <div class="hero-content">
        <h1>Compassionate Funeral Services</h1>
        <p>Providing dignified care and support during difficult times</p>
        <div class="hero-actions">
          <KButton variant="primary" @click="scrollToServices">
            Our Services
          </KButton>
          <KButton variant="outline" @click="scrollToAbout">
            About Us
          </KButton>
        </div>
      </div>
      <div class="hero-image">
        <!-- Hero image would go here -->
      </div>
    </section>

    <section class="stats-section">
      <div class="container">
        <div class="stats-grid">
          <div class="stat-card">
            <div class="stat-number">{{ stats.yearsOfExperience }}</div>
            <div class="stat-label">Years of Experience</div>
          </div>
          <div class="stat-card">
            <div class="stat-number">{{ stats.familiesServed }}</div>
            <div class="stat-label">Families Served</div>
          </div>
          <div class="stat-card">
            <div class="stat-number">24/7</div>
            <div class="stat-label">Available Support</div>
          </div>
          <div class="stat-card">
            <div class="stat-number">{{ stats.satisfactionRate }}%</div>
            <div class="stat-label">Satisfaction Rate</div>
          </div>
        </div>
        <div class="connection-status-indicator">
          <ConnectionStatus />
          <span class="status-text">{{ connectionStatus }}</span>
          <span class="stat-indicator" v-if="statsUpdatedRecently" class="updated-indicator">●</span>
        </div>
      </div>
    </section>

    <section class="services-section">
      <div class="container">
        <h2 class="section-title">Our Services</h2>
        <p class="section-subtitle">Comprehensive funeral and memorial services tailored to your needs</p>
        
        <div class="services-grid">
          <div v-for="service in services" :key="service.id" class="service-card">
            <div class="service-icon">
              {{ service.icon }}
            </div>
            <h3 class="service-title">{{ service.title }}</h3>
            <p class="service-description">{{ service.description }}</p>
            <KButton variant="secondary" size="sm" @click="scrollToServices">
              Learn More
            </KButton>
          </div>
        </div>
      </div>
    </section>

    <section class="testimonials-section">
      <div class="container">
        <h2 class="section-title">What Families Say</h2>
        <p class="section-subtitle">Hear from those we've had the privilege to serve</p>
        
        <div class="testimonials-slider">
          <div v-for="testimonial in testimonials" :key="testimonial.id" class="testimonial-card">
            <p class="testimonial-text">"{{ testimonial.text }}"</p>
            <div class="testimonial-author">
              <div class="author-info">
                <h4 class="author-name">{{ testimonial.author }}</h4>
                <p class="author-location">{{ testimonial.location }}</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <section class="cta-section">
      <div class="container">
        <h2>Ready to Plan a Service?</h2>
        <p>Our compassionate team is here to help you every step of the way.</p>
        <div class="cta-actions">
          <KButton variant="primary" @click="showContactForm = true">
            Contact Us Now
          </KButton>
          <KButton variant="outline" @click="showPricing = true">
            View Pricing
          </KButton>
        </div>
      </div>
    </section>
  </div>

  <!-- Contact Form Modal -->
  <KDialog v-model:show="showContactForm" title="Contact Us" width="500px">
    <div class="dialog-content">
      <form @submit.prevent="submitContactForm">
        <div class="form-group">
          <label for="name" class="form-label">Full Name *</label>
          <KInput
            v-model="form.name"
            id="name"
            placeholder="Enter your full name"
            required
          />
        </div>
        
        <div class="form-group">
          <label for="email" class="form-label">Email Address *</label>
          <KInput
            type="email"
            v-model="form.email"
            id="email"
            placeholder="Enter your email"
            required
          />
        </div
        
        <div class="form-group">
          <label for="phone" class="form-label">Phone Number *</label>
          <KInput
            v-model="form.phone"
            id="phone"
            placeholder="Enter your phone number"
            required
          />
        </div>
        
        <div class="form-group">
          <label for="serviceType" class="form-label">Service Type *</label>
          <KSelect
            v-model="form.serviceType"
            id="serviceType"
            :options="serviceTypes"
            placeholder="Select service type"
            required
          />
        </div>
        
        <div class="form-group">
          <label for="message" class="form-label">Message</p>
          <KTextarea
            v-model="form.message"
            id="message"
            placeholder="How can we help you?"
            rows="4"
          />
        </div
        
        <div class="form-group">
          <KInput
            type="checkbox"
            v-model="form.subscribe"
            id="subscribe"
          />
          <label for="subscribe" class="form-label">
            Subscribe to our newsletter for helpful resources
          </label>
        </div>
      </form>
    </div>
    
    <template #footer>
      <KButton variant="secondary" @click="showContactForm = false">
        Cancel
      </KButton>
      <KButton 
        variant="primary" 
        @click="submitContactForm"
        :loading="submitting"
      >
        Send Message
      </KButton>
    </template>
  </KDialog>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount, computed } from 'vue';
import { KButton, KInput, KSelect, KTextarea, KDialog } from '@khet360/ui-shared';
import { PublicSiteService } from '@/services/publicSiteService';
import { signalRService } from '@/services/signalRService';
import { ConnectionStatus } from '@/components/ConnectionStatus.vue';

// Sample data for the homepage
const services = ref([
  { 
    id: 1, 
    icon: '⚰️', 
    title: 'Traditional Funerals', 
    description: 'Full-service funeral arrangements with viewing, ceremony, and burial.'
  },
  { 
    id: 2, 
    icon: '🔥', 
    title: 'Cremation Services', 
    description: 'Respectful cremation options with memorial services.'
  },
  { 
    id: 3, 
    icon: '💐', 
    title: 'Memorial Services', 
    description: 'Personalized memorial celebrations without the body present.'
  },
  { 
    id: 4, 
    icon: '🕯️', 
    title: 'Pre-Planning', 
    description: 'Plan ahead to relieve the burden on your loved ones.'
  }
]);

const testimonials = ref([]);
const serviceTypes = [
  'Traditional Funeral',
  'Cremation Service',
  'Memorial Service',
  'Pre-Planning Consultation',
  'Grief Support'
];

const form = ref({
  name: '',
  email: '',
  phone: '',
  serviceType: '',
  message: '',
  subscribe: false
});

const showContactForm = ref(false);
const showPricing = ref(false);
const submitting = ref(false);

// Connection status
const connectionStatus = ref('Checking...');
const isConnected = ref(false);
const statsUpdatedRecently = ref(false);

// Statistics
const stats = ref({
  yearsOfExperience: 0,
  familiesServed: 0,
  satisfactionRate: 0
});

// Real-time updates
const handleStatsUpdate = (statsData: any) => {
  // Update stats in real-time
  stats.value = { ...stats.value, ...statsData };
  statsUpdatedRecently.value = true;
  
  // Reset the update indicator after 5 seconds
  setTimeout(() => {
    statsUpdatedRecently.value = false;
  }, 5000);
  
  notificationService.addNotification({
    title: 'Service Update',
    message: 'Our service statistics have been updated',
    type: 'info'
  });
};

const handleNewTestimonial = (testimonial: any) => {
  // Add new testimonial to the list
  testimonials.value = [...testimonials.value, testimonial];
  
  notificationService.addNotification({
    title: 'New Testimonial',
    message: 'A new family testimonial has been added',
    type: 'success'
  });
};

const handleServiceUpdated = (service: any) => {
  // Update service in the list
  const index = services.value.findIndex(s => s.id === service.id);
  if (index !== -1) {
    services.value[index] = { ...services.value[index], ...service };
    notificationService.addNotification({
      title: 'Service Updated',
      message: `Service "${service.title}" has been updated`,
      type: 'info'
    });
  }
};

const fetchInitialStats = async () => {
  try {
    // Fetch statistics from API
    const statsData = await PublicSiteService.getStatistics();
    stats.value = statsData;
    connectionStatus.value = 'Connected';
    isConnected.value = true;
  } catch (err) {
    connectionStatus.value = 'Disconnected';
    isConnected.value = false;
    console.error('Error fetching initial stats:', err);
    // Fallback to mock data
    stats.value = {
      yearsOfExperience: 10,
      familiesServed: 500,
      satisfactionRate: 98
    };
  }
};

const fetchTestimonials = async () => {
  try {
    const testimonialsData = await PublicSiteService.getTestimonials();
    testimonials.value = testimonialsData;
  } catch (err) {
    console.error('Error fetching testimonials:', err);
    // Fallback to mock data
    testimonials.value = [
      {
        id: 1,
        text: 'The team at Khet-360 handled everything with such care and professionalism during our difficult time. We felt supported every step of the way.',
        author: 'Johnson Family',
        location: 'Johannesburg'
      },
      {
        id: 2,
        text: 'From the first call to the final service, the compassion and attention to detail was exceptional. Highly recommend their services.',
        author: 'Williams Family',
        location: 'Cape Town'
      },
      {
        id: 3,
        text: 'They made a very difficult process as smooth as possible. The staff was kind, knowledgeable, and truly cared about our family.',
        author: 'Mkhize Family',
        location: 'Durban'
      }
    ];
  }
};

const fetchServices = async () => {
  try {
    const servicesData = await PublicSiteService.getServices();
    services.value = servicesData;
  } catch (err) {
    console.error('Error fetching services:', err);
    // Keep the default services if API fails
  }
};

onMounted(() => {
  // Fetch initial data
  fetchInitialStats();
  fetchTestimonials();
  fetchServices();
  
  // Set up SignalR listeners for real-time updates
  const statsCleanup = PublicSiteService.subscribeToStatisticsUpdates(
    handleStatsUpdate
  );
  
  const testimonialCleanup = PublicSiteService.subscribeToNewTestimonials(
    handleNewTestimonial
  );
  
  const serviceCleanup = PublicSiteService.subscribeToServiceUpdates(
    handleServiceUpdated
  );
  
  // Start SignalR connection
  signalRService.start().catch(err => {
    console.error('Failed to start SignalR connection:', err);
    connectionStatus.value = 'Connection Error';
    isConnected.value = false;
  });
});

onBeforeUnmount(() => {
  // Cleanup SignalR subscriptions
  if (statsCleanup) statsCleanup();
  if (testimonialCleanup) testimonialCleanup();
  if (serviceCleanup) serviceCleanup();
  
  // Stop SignalR connection
  signalRService.stop();
});

// Helper methods
const scrollToServices = () => {
  document.querySelector('.services-section')?.scrollIntoView({ behavior: 'smooth' });
};

const scrollToAbout = () => {
  document.querySelector('.about-section')?.scrollIntoView({ behavior: 'smooth' };
};

const submitContactForm = async () => {
  submitting.value = true;
  try {
    // Submit the contact form
    await PublicSiteService.submitContactForm(form.value);
    
    // Show success message
    alert('Thank you for your message! We will get back to you shortly.');
    showContactForm.value = false;
    
    // Reset form
    form.value = {
      name: '',
      email: '',
      phone: '',
      serviceType: '',
      message: '',
      subscribe: false
    };
  } catch (err) {
    alert('There was an error submitting your message. Please try again.');
    console.error('Error submitting contact form:', err);
  } finally {
    submitting.value = false;
  }
};
</script>

<script setup lang="ts">
</script>

<style scoped>
.home-page {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.hero {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  text-align: center;
  padding: 6rem 2rem;
  position: relative;
  overflow: hidden;
}

.hero-content {
  max-width: 800px;
  margin: 0 auto;
  z-index: 2;
}

.hero h1 {
  font-size: 3rem;
  margin-bottom: 1.5rem;
  font-weight: 700;
}

.hero p {
  font-size: 1.25rem;
  margin-bottom: 2rem;
  opacity: 0.9;
}

.hero-actions {
  display: flex;
  gap: 1rem;
  justify-content: center;
  flex-wrap: wrap;
}

.hero-image {
  position: absolute;
  bottom: 0;
  left: 0;
  width: 100%;
  height: 300px;
  background: url('/images/hero-bg.jpg') center/cover no-repeat;
  opacity: 0.3;
  z-index: 1;
}

.stats-section {
  background-color: #f8f9fa;
  padding: 4rem 2rem;
  text-align: center;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 2rem;
  max-width: 1000px;
  margin: 0 auto;
}

.stat-card {
  background: white;
  padding: 2rem;
  border-radius: 12px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  text-align: center;
  transition: transform 0.3s;
}

.stat-card:hover {
  transform: translateY(-5px);
}

.stat-number {
  font-size: 3rem;
  font-weight: 700;
  color: #3498db;
  display: block;
  margin-bottom: 0.5rem;
}

.stat-label {
  font-size: 1rem;
  color: #7f8c8d;
  font-weight: 500;
}

.services-section {
  padding: 4rem 2rem;
}

.section-title {
  text-align: center;
  font-size: 2.5rem;
  margin-bottom: 1rem;
  color: #2c3e50;
}

.section-subtitle {
  text-align: center;
  font-size: 1.25rem;
  margin-bottom: 3rem;
  color: #7f8c8d;
  max-width: 800px;
  margin-left: auto;
  margin-right: auto;
}

.services-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 2rem;
  max-width: 1200px;
  margin: 0 auto;
}

.service-card {
  background: white;
  border-radius: 12px;
  padding: 2rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  text-align: center;
  transition: transform 0.3s, box-shadow 0.3s;
}

.service-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 8px 15px rgba(0, 0, 0, 0.15);
}

.service-icon {
  font-size: 3rem;
  margin-bottom: 1.5rem;
  color: #3498db;
}

.service-title {
  font-size: 1.5rem;
  margin-bottom: 1rem;
  font-weight: 600;
  color: #2c3e50;
}

.service-description {
  color: #7f8c8d;
  line-height: 1.6;
  margin-bottom: 1.5rem;
}

.testimonials-section {
  background-color: #f8f9fa;
  padding: 4rem 2rem;
}

.testimonials-slider {
  display: flex;
  gap: 2rem;
  overflow-x: auto;
  padding: 2rem 0;
  scrollbar-width: thin;
}

.testimonials-slider::-webkit-scrollbar {
  height: 8px;
}

.testimonials-slider::-webkit-scrollbar-thumb {
  background-color: #3498db;
  border-radius: 4px;
}

.testimonial-card {
  background: white;
  border-radius: 12px;
  padding: 2rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  flex: 0 0 320px;
  transition: transform 0.3s;
}

.testimonial-card:hover {
  transform: translateY(-5px);
}

.testimonial-text {
  font-style: italic;
  color: #2c3e50;
  margin-bottom: 1.5rem;
  line-height: 1.6;
}

.testimonial-author {
  text-align: right;
}

.author-name {
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 0.5rem;
}

.author-location {
  color: #7f8c8d;
  font-size: 0.9rem;
}

.cta-section {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  text-align: center;
  padding: 4rem 2rem;
}

.cta-section h2 {
  font-size: 2.5rem;
  margin-bottom: 1.5rem;
}

.cta-section p {
  font-size: 1.25rem;
  margin-bottom: 2rem;
  opacity: 0.9;
}

.cta-actions {
  display: flex;
  gap: 1.5rem;
  justify-content: center;
  flex-wrap: wrap;
}

.connection-status-indicator {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-top: 2rem;
}

.status-text {
  font-size: 0.875rem;
  font-weight: 500;
}

.updated-indicator {
  color: #10b981;
  font-size: 1rem;
  animation: pulse 2s infinite;
}

@keyframes pulse {
  0% {
    opacity: 1;
  }
  50% {
    opacity: 0.5;
  }
  100% {
    opacity: 1;
  }
}

/* Dialog styles */
.dialog-content {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-label {
  font-weight: 600;
  font-size: 0.875rem;
  margin-bottom: 0.25rem;
}

@media (max-width: 768px) {
  .hero {
    padding: 4rem 2rem;
  }
  
  .hero h1 {
    font-size: 2.5rem;
  }
  
  .stats-grid {
    grid-template-columns: 1fr;
  }
  
  .services-grid {
    grid-template-columns: 1fr;
  }
  
  .cta-actions {
    flex-direction: column;
    align-items: center;
  }
}
</style>