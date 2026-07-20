import {
	Children,
	cloneElement,
	isValidElement,
	ReactElement,
	ReactNode,
} from "react"
import { StepContent } from "./StepContent"
import { StepHeader } from "./StepHeader"

type StepChildProps = {
	stepId?: number | string
	children?: ReactNode
}

export const Step = ({
	children,
	stepId,
}: {
	children?: ReactNode
	stepId?: number | string
}) => {
	return (
		<>
			{Children.map(children, (child) => {
				if (!isValidElement(child)) {
					return child
				}

				if (child.type === StepHeader || child.type === StepContent) {
					return cloneElement(child as ReactElement<StepChildProps>, {
						stepId,
					})
				}

				return child
			})}
		</>
	)
}
