interface PayoutsDataOptions {
  type: string;
  account_id: string;
  success_callback: (data: any) => void;
  error_callback: (error: any) => void;
  customization?: {
    colors?: {
      [key: string]: string;
    };
  };
}

interface PayoutsData {
  new (options: PayoutsDataOptions): any;
}

interface Window {
  PayoutsData: PayoutsData;
}
